using System.Diagnostics;
using MusicDownloader.Models;

namespace MusicDownloader.Services.Downloaders;

public class YoutubeDownloader
{
    private readonly string _musicFolder;

    public YoutubeDownloader(string musicFolder)
    {
        _musicFolder = musicFolder;
    }

    private async Task Loading(CancellationToken token)
    {
        string[] frames = { "|", "/", "-", "\\" };
        int i = 0;

        const string message = "Reading URL... (This could take a while)";

        try
        {
            while (!token.IsCancellationRequested)
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
            Console.Write("\r" + new string(' ', message.Length + 3) + "\r");
        }
    }

    public bool CanHandle(string url)
    {
        return url.Contains("youtube.com",

        StringComparison.OrdinalIgnoreCase) || url.Contains("youtu.be",
        
        StringComparison.OrdinalIgnoreCase);
    }

    public async Task<DownloadResult> DownloadAsync(string url)
    {
        string? musicName;

        using (var cts = new CancellationTokenSource())
        {
            Task loadingTask = Loading(cts.Token);
            
            try
            {
                musicName = await GetVideoTitleAsync(url);
            }
            finally
            {
                cts.Cancel();
                await loadingTask;
            }
        }

        Console.WriteLine($"Starting download for: \"{musicName ?? url}\"");

        var startInfo = new ProcessStartInfo
        {
            FileName = "yt-dlp",
            WorkingDirectory = Path.GetFullPath(_musicFolder),
            UseShellExecute = false
        };

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

            if (process is null)
            {
                return new DownloadResult(
                    false,
                    ErrorMessage: "Could not start yt-dlp."
                );
            }

            await process.WaitForExitAsync();
            
            if (process.ExitCode == 0)
            {
                return new DownloadResult(true);
            }
            
            return new DownloadResult(
                false,
                ErrorMessage: $"yt-dlp exited with code {process.ExitCode}"
            );
        }
        catch (Exception ex)
        {
            return new DownloadResult(
                false,
                ErrorMessage: ex.Message
            );
        }
    }

    private async Task<string?> GetVideoTitleAsync(string url)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "yt-dlp",
            WorkingDirectory = Path.GetFullPath(_musicFolder),
            UseShellExecute = false,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            CreateNoWindow = true
        };

        startInfo.ArgumentList.Add("--get-title");
        startInfo.ArgumentList.Add(url);

        using var process = Process.Start(startInfo);

        if (process is null)
            return null;

        string title = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        return process.ExitCode == 0 ? title.Trim() : null;
    }
}
