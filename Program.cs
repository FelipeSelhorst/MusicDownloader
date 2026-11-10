using MusicDownloader.Models;
using MusicDownloader.Services;
using MusicDownloader.Services.Downloaders;

const string MusicFolder = "songs";

Directory.CreateDirectory(MusicFolder);

var downloader = new YoutubeDownloader(MusicFolder);

var musicLibrary = new MusicLibrary(MusicFolder);

var playlistService = new PlaylistService(
    Path.Combine(MusicFolder, "songs.json"));

static async Task Loading(CancellationToken token)
{
    string[] frames = { "|", "/", "-", "\\" };
    int i = 0;

    const string message = "Reading URL... (This could take a while)";

    try
    {
        while (true)
        {
            Console.Write($"\r{frames[i++ % frames.Length]} {message}");
            await Task.Delay(150, token);
        }
    }
    catch (OperationCanceledException)
    {
        // Loading finished, exit the loop
    }
    finally
    {
        Console.WriteLine("\r" + new string(' ', message.Length + 3) + "\r");
    }
}

Console.WriteLine("================================");
Console.WriteLine("       MUSIC DOWNLOADER");
Console.WriteLine("================================");

var urlHandler = new UrlHandler();

while (true)
{

    // User input
    Console.Write("\nPaste a URL (\"exit\" to quit): ");

    string url = Console.ReadLine()?.Trim() ?? string.Empty;

    if (url.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;


    // Verify URL
    var urlInfo = urlHandler.GetUrlInfo(url);

    if (urlInfo is null)
    {
        Console.WriteLine("Unsupported URL.");
        continue;
    }

    Console.WriteLine($"\nPlatform: {urlInfo.Platform}");
    Console.WriteLine($"Resource: {urlInfo.ResourceType}");
    Console.WriteLine($"ID: {urlInfo.Id}\n");

    using var cts = new CancellationTokenSource();

    Task loadingTask = Loading(cts.Token);

    DownloadResult result;

    try
    {
        result = await downloader.DownloadAsync(url);
    }
    finally
    {
        cts.Cancel();
        await loadingTask;
    }

    if (!result.Success)
    {
        Console.WriteLine($"Download failed: {result.ErrorMessage}");
        Console.WriteLine($"Continuing with existing songs...");
    }

    // Download the resource
    if (!string.IsNullOrWhiteSpace(url))
    {
        //var result = await downloader.DownloadAsync(url);

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
}

// Display Playlist
static void DisplayPlaylist(List<string> songs)
{
    Console.WriteLine(
        $"\n--- Current Player Playlist ({songs.Count} songs) ---");

    for (int i = 0; i < songs.Count(); i++)
    {
        Console.WriteLine($"{i + 1}. {songs[i]}");
    }
}

// End the program
Console.WriteLine("\nProgram finished!");

if (!Console.IsInputRedirected)
{
    Console.WriteLine("Press any key to exit...");
    Console.ReadKey(true);
}