using System.Web;

namespace MusicDownloader.Services;

public class UriInfo
{
    public string Host { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

public class UriHandler
{
    public UriInfo? UrlHandler(string url)
    {
        Uri UrlUri = new Uri(url);

        var query = HttpUtility.ParseQueryString(UrlUri.Query);

        return new UriInfo
        {
            Host = UrlUri.Host,
            Resource = UrlUri.AbsolutePath.TrimStart('/').TrimEnd('/'),
            Id = query["v"] ?? query["list"] ?? string.Empty
        };
    }
}
