using System.Diagnostics;
using System.Text.Json;

const string MusicFolder = "songs";

string jsonPath = Path.Combine(MusicFolder, "songs.json");

Directory.CreateDirectory(MusicFolder);

DownloadResult? downloadResult = null;

Console.Write("Please paste your URL: ");
string url = Console.ReadLine()?.Trim() ?? "";

if (!string.IsNullOrWhiteSpace(url))
{
    downloadResult = DownloadYT(url);

    if (downloadResult?.Success == false)
    {
        Console.WriteLine("Download failed. Continuing with existing songs...");
    }
}

List<string> currentSongs = ScanMusicFolder();

SyncPlaylist(currentSongs);

Console.WriteLine(
    $"\n--- Current Player Playlist ({currentSongs.Count} songs) ---");

for (int i = 0; i < currentSongs.Count; i++)
{
    Console.WriteLine($"{i + 1}. {currentSongs[i]}");
}

Console.WriteLine("\nDone!");

if (!Console.IsInputRedirected)
{
    Console.WriteLine("Press any key to exit... ");
    Console.ReadKey(true);
}

DownloadResult? DownloadYT(string url)
{
    Console.WriteLine($"Starting download for \"{url}\"");

    var startInfo = new ProcessStartInfo
    {
        FileName = "yt-dlp",
        WorkingDirectory = Path.GetFullPath(MusicFolder),
        UseShellExecute = false
    };

    // Use yt-dlp's own audio extraction and FFmpeg conversion.
    string[] arguments =
    {
        "--extract-audio",
        "--audio-format", "mp3",
        "--audio-quality", "192K",
        "--windows-filenames",
        "--trim-filenames", "180",
        "--output", "%(title)s.%(ext)s",
        url
    };

    foreach (string argument in arguments)
    {
        startInfo.ArgumentList.Add(argument);
    }

    try
    {
        using Process? process = Process.Start(startInfo);

        if (process == null)
        {
            Console.WriteLine("Could not start yt-dlp.");
            return null;
        }

        process.WaitForExit();

        if (process.ExitCode == 0)
        {
            Console.WriteLine("Download and conversion complete!");
            return new DownloadResult(true);
        }

        Console.WriteLine(
            $"yt-dlp failed with exit code {process.ExitCode}.");

        return new DownloadResult(false);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Download failed: {ex.Message}");
        return new DownloadResult(false);
    }
}

List<string> ScanMusicFolder()
{
    string[] validExtensions =
    {
        ".mp3", ".wav", ".mp4", ".flac"
    };

    return Directory
        .EnumerateFiles(MusicFolder)
        .Where(file => validExtensions.Contains(
            Path.GetExtension(file),
            StringComparer.OrdinalIgnoreCase))
        .Select(Path.GetFileName)
        .Where(name => name != null)
        .Select(name => name!)
        .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
        .ToList();
}

PlaylistData GetPlaylist()
{
    if (!File.Exists(jsonPath))
    {
        return new PlaylistData();
    }

    try
    {
        string json = File.ReadAllText(jsonPath);

        return JsonSerializer.Deserialize<PlaylistData>(json)
            ?? new PlaylistData();
    }
    catch (JsonException)
    {
        Console.WriteLine(
            "Warning: songs.json is invalid. Starting a new playlist.");

        return new PlaylistData();
    }
    catch (IOException ex)
    {
        Console.WriteLine($"Could not read playlist: {ex.Message}");
        return new PlaylistData();
    }
}

void SyncPlaylist(List<string> songs)
{
    PlaylistData data = GetPlaylist();

    var existingFiles = new HashSet<string>(
        data.Playlist.Select(song => song.File),
        StringComparer.OrdinalIgnoreCase
    );

    foreach (string filename in songs)
    {
        if (existingFiles.Add(filename))
        {
            data.Playlist.Add(new Song
            {
                Title = Path.GetFileNameWithoutExtension(filename),
                File = filename
            });
        }
    }
    var options = new JsonSerializerOptions
    {
        WriteIndented = true
    };

    File.WriteAllText(
        jsonPath,
        JsonSerializer.Serialize(data, options));
}

class PlaylistData
{
    public List<Song> Playlist { get; set; } = new();
}

class Song
{
    public string Title { get; set; } = "";
    public string File { get; set; } = "";
}

record DownloadResult(bool Success);