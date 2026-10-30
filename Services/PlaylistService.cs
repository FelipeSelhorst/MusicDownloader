using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using MusicDownloader.Models;

namespace MusicDownloader.Services;

public class  PlaylistService
{
    private readonly string _jsonPath;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true
    };

    public PlaylistService(string jsonPath)
    {
        _jsonPath = jsonPath;
    }

    public void Sync(List<string> songs)
    {
        PlaylistData playlist = Load();

        var existingFiles = new HashSet<string>(
            playlist.Playlist.Select(song => song.File),
            StringComparer.OrdinalIgnoreCase);

        foreach (string filename in songs)
        {
            if (!existingFiles.Add(filename))
            {
                continue;
            }

            playlist.Playlist.Add(new Song {
                Title = Path.GetFileNameWithoutExtension(filename),
                File = filename
            });
        }

        Save(playlist);
    }

    private PlaylistData Load()
    {
        if (!File.Exists(_jsonPath))
        {
            return new PlaylistData();
        }
        try
        {
            string json = File.ReadAllText(_jsonPath);
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

    private void Save(PlaylistData playlist)
    {
        string json = JsonSerializer.Serialize(
            playlist,
            _jsonOptions);

        File.WriteAllText(_jsonPath, json);
    }
}
