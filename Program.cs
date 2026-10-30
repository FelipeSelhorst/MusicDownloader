using MusicDownloader.Services;
using MusicDownloader.Services.Downloaders;

const string MusicFolder = "songs";

Directory.CreateDirectory(MusicFolder);

var downloader = new YoutubeDownloader(MusicFolder);

var musicLibrary = new MusicLibrary(MusicFolder);

var playlistService = new PlaylistService(
    Path.Combine(MusicFolder, "songs.json"));

Console.Write("Please paste your URL: ");

string url = Console.ReadLine()?.Trim() ?? string.Empty;

if (!string.IsNullOrWhiteSpace(url))
{
    var result = await downloader.DownloadAsync(url);

    if (!result.Success)
    {
        Console.WriteLine(
            $"Download failed: {result.ErrorMessage}");

        Console.WriteLine(
            "Continuing with existing songs...");
    }
}
else
{
    Console.WriteLine("No URL provided. Exiting.");
}

List<string> songs = musicLibrary.GetSongs();

playlistService.Sync(songs);

DisplayPlaylist(songs);

Console.WriteLine("\nProgram finished!");

if (!Console.IsInputRedirected)
{
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey(true);
}

static void DisplayPlaylist(List<string> songs)
{
    Console.WriteLine(
        $"\n--- Current Player Playlist ({songs.Count} songs) ---");

    for (int i = 0; i < songs.Count(); i++)
    {
        Console.WriteLine($"{i + 1}. {songs[i]}");
    }
}