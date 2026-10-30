using System;
using System.Collections.Generic;
using System.Text;

namespace MusicDownloader.Services;

public class MusicLibrary
{
    private readonly string _musicFolder;

    private static readonly string[] ValidExtensions =
    {
        ".mp3",
        ".wav",
        ".mp4",
        ".flac"
    };

    public MusicLibrary(string musicFolder)
    {
        _musicFolder = musicFolder;
    }

    public List<string> GetSongs()
    {
        return Directory
            .EnumerateFiles(_musicFolder)
            .Where(IsValidAudioFile)
            .Select(Path.GetFileName)
            .Where(name => name != null)
            .Select(name => name!)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static bool IsValidAudioFile(string file)
    {
        string extension = Path.GetExtension(file);

        return ValidExtensions.Contains(
            extension,
            StringComparer.OrdinalIgnoreCase);
    }
}
