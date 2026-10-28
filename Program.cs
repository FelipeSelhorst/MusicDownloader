
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
}

List<string> currentSongs = ScanMusicFolder();

foreach (string song in currentSongs)
{
    string title = Path.GetFileNameWithoutExtension(song);
    AddSong(title, song);
}

Console.WriteLine(
    $"\n--- Current Player Playlist ({currentSongs.Count} songs) ---");

for (int i = 0; i < currentSongs.Count; i++)
{
    Console.WriteLine($"{i + 1}. {currentSongs[i]}");
}

Console.WriteLine("\nDone!");

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

void AddSong(string songTitle, string filename)
{
    PlaylistData data = GetPlaylist();

    bool alreadyExists = data.Playlist.Any(song =>
        string.Equals(
            song.File,
            filename,
            StringComparison.OrdinalIgnoreCase));

    if (!alreadyExists)
    {
        data.Playlist.Add(new Song
        {
            Title = songTitle,
            File = filename
        });
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