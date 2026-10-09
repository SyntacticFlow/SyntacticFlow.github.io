using System.Text.RegularExpressions;

namespace Portfolio.Web.Services;

public static partial class Media
{
    [GeneratedRegex(@"(?:youtube\.com/(?:watch\?(?:.*&)?v=|embed/|shorts/)|youtu\.be/)(?<id>[\w-]{11})")]
    private static partial Regex YouTube();

    [GeneratedRegex(@"vimeo\.com/(?:video/)?(?<id>\d+)")]
    private static partial Regex Vimeo();

    private static readonly string[] VideoExtensions = [".mp4", ".webm", ".mov", ".m4v"];

    public static bool IsRelative(string url) =>
        !(url.StartsWith('/') || url.StartsWith('#') || url.Contains("://") || url.StartsWith("mailto:") || url.StartsWith("data:"));

    /// <summary>Turns a front-matter path into a URL; bare file names live in the item's media folder.</summary>
    public static string? Resolve(string? path, string mediaBase) =>
        string.IsNullOrWhiteSpace(path) ? null
        : IsRelative(path) ? $"{mediaBase}/{path}"
        : path;

    public static VideoEmbed? ParseVideo(string? video, string mediaBase)
    {
        if (string.IsNullOrWhiteSpace(video)) return null;

        if (YouTube().Match(video) is { Success: true } yt)
            return new(VideoKind.YouTube, $"https://www.youtube-nocookie.com/embed/{yt.Groups["id"].Value}");

        if (Vimeo().Match(video) is { Success: true } vm)
            return new(VideoKind.Vimeo, $"https://player.vimeo.com/video/{vm.Groups["id"].Value}");

        var path = video.Split('?', '#')[0];
        if (VideoExtensions.Any(ext => path.EndsWith(ext, StringComparison.OrdinalIgnoreCase)))
            return new(VideoKind.File, Resolve(video, mediaBase)!);

        return null;
    }

    public static string MimeType(string url) => Path.GetExtension(url.Split('?', '#')[0]).ToLowerInvariant() switch
    {
        ".webm" => "video/webm",
        ".mov" => "video/quicktime",
        _ => "video/mp4",
    };
}
