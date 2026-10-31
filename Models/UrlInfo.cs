using MusicDownloader.Enums;

namespace MusicDownloader.Models;

public record UrlInfo
(
    MusicPlatform Platform,
    MusicResourceType ResourceType,
    string Id
);