namespace Portfolio.Web.Services;

public sealed record MediaItem(string Src, string? Caption);

public enum VideoKind { YouTube, Vimeo, File }

public sealed record VideoEmbed(VideoKind Kind, string Url);

public sealed record Project
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public string Summary { get; init; } = "";
    public DateOnly Date { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public IReadOnlyList<string> Tech { get; init; } = [];
    public string? Cover { get; init; }
    public IReadOnlyList<MediaItem> Gallery { get; init; } = [];
    public VideoEmbed? Video { get; init; }
    public string? Poster { get; init; }
    public string? RepoUrl { get; init; }
    public string? LiveUrl { get; init; }
    public bool Featured { get; init; }
    public int Order { get; init; }
    public required string Html { get; init; }
}

public sealed record Post
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public string Summary { get; init; } = "";
    public DateOnly Date { get; init; }
    public IReadOnlyList<string> Tags { get; init; } = [];
    public string? Cover { get; init; }
    public bool Draft { get; init; }
    public int ReadingMinutes { get; init; }
    public required string Html { get; init; }
}

public sealed record Page
{
    public required string Slug { get; init; }
    public required string Title { get; init; }
    public string Summary { get; init; } = "";
    public required string Html { get; init; }
}
