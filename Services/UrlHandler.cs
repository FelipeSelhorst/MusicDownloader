using System.Web;
using MusicDownloader.Enums;
using MusicDownloader.Models;

namespace MusicDownloader.Services;

public class UrlHandler
{
    public UrlInfo? GetUrlInfo(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            return null;

        string host = uri.Host;
        string path = uri.AbsolutePath;

        // YouTube
        if (host == "youtube.com" ||
            host == "www.youtube.com" ||
            host == "youtu.be")
        {
            var query = HttpUtility.ParseQueryString(uri.Query);

            if (host == "youtu.be")
            {
                string id = path.Trim('/');

                return string.IsNullOrEmpty(id)
                    ? null
                    : new UrlInfo(
                        MusicPlatform.YouTube,
                        MusicResourceType.Video,
                        id);
            }

            if (path == "/watch" && !string.IsNullOrEmpty(query["v"]))
            {
                return new UrlInfo(
                    MusicPlatform.YouTube,
                    MusicResourceType.Video,
                    query["v"]!);
            }

            if (path == "/playlist" && !string.IsNullOrEmpty(query["list"]))
            {
                return new UrlInfo(
                    MusicPlatform.YouTube,
                    MusicResourceType.Playlist,
                    query["list"]!);
            }
        }

        // Spotify
        if (host == "open.spotify.com")
        {
            string[] segments = path.Trim('/').Split('/');

            if (segments.Length != 2)
                return null;

            MusicResourceType resourceType = segments[0] switch
            {
                "track" => MusicResourceType.Track,
                "playlist" => MusicResourceType.Playlist,
                "album" => MusicResourceType.Album,
                _ => MusicResourceType.Unknown
            };

            if (resourceType == MusicResourceType.Unknown)
                return null;

            return new UrlInfo(
                MusicPlatform.Spotify,
                resourceType,
                segments[1]);
        }

        return null;
    }
}