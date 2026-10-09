namespace Portfolio.Web.Services;

public sealed record ContentSnapshot(
    IReadOnlyList<Project> Projects,
    IReadOnlyList<Post> Posts,
    IReadOnlyDictionary<string, Page> Pages)
{
    public static readonly ContentSnapshot Empty = new([], [], new Dictionary<string, Page>());
}

/// <summary>
/// Reads <c>projects/*.md</c>, <c>posts/*.md</c> and <c>pages/*.md</c> from a content folder.
/// A file that fails to parse is logged and skipped so one typo can't take the site down.
/// </summary>
public sealed class ContentLoader(MarkdownRenderer markdown, ILogger<ContentLoader> logger)
{
    public ContentSnapshot Load(string root)
    {
        var projects = ReadAll(Path.Combine(root, "projects"), ParseProject)
            .OrderBy(p => p.Order == 0 ? int.MaxValue : p.Order)
            .ThenByDescending(p => p.Date)
            .ToList();

        var posts = ReadAll(Path.Combine(root, "posts"), ParsePost)
            .OrderByDescending(p => p.Date)
            .ThenBy(p => p.Title)
            .ToList();

        var pages = ReadAll(Path.Combine(root, "pages"), ParsePage)
            .ToDictionary(p => p.Slug, StringComparer.OrdinalIgnoreCase);

        logger.LogInformation("Loaded {Projects} projects, {Posts} posts and {Pages} pages from {Root}",
            projects.Count, posts.Count, pages.Count, root);

        return new(projects, posts, pages);
    }

    private IEnumerable<T> ReadAll<T>(string folder, Func<string, string, T> parse)
    {
        if (!Directory.Exists(folder)) yield break;

        foreach (var file in Directory.EnumerateFiles(folder, "*.md").Order(StringComparer.Ordinal))
        {
            var slug = Path.GetFileNameWithoutExtension(file).ToLowerInvariant();
            T item;
            try
            {
                item = parse(slug, File.ReadAllText(file));
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Skipping {File}: it could not be parsed", file);
                continue;
            }
            yield return item;
        }
    }

    public Project ParseProject(string slug, string text)
    {
        var (meta, body) = FrontMatter.Parse<ProjectMeta>(text);
        var media = $"/media/projects/{slug}";

        return new Project
        {
            Slug = slug,
            Title = meta.Title ?? slug,
            Summary = meta.Summary ?? "",
            Date = meta.Date is { } d ? DateOnly.FromDateTime(d) : default,
            Tags = meta.Tags ?? [],
            Tech = meta.Tech ?? [],
            Cover = Media.Resolve(meta.Cover, media),
            Gallery = (meta.Gallery ?? []).Select(g => ToMediaItem(g, media)).OfType<MediaItem>().ToList(),
            Video = Media.ParseVideo(meta.Video, media),
            Poster = Media.Resolve(meta.Poster, media),
            RepoUrl = meta.Repo,
            LiveUrl = meta.Live,
            Featured = meta.Featured,
            Order = meta.Order,
            Html = markdown.Render(body, media),
        };
    }

    public Post ParsePost(string slug, string text)
    {
        var (meta, body) = FrontMatter.Parse<PostMeta>(text);
        var media = $"/media/posts/{slug}";

        return new Post
        {
            Slug = slug,
            Title = meta.Title ?? slug,
            Summary = meta.Summary ?? "",
            Date = meta.Date is { } d ? DateOnly.FromDateTime(d) : default,
            Tags = meta.Tags ?? [],
            Cover = Media.Resolve(meta.Cover, media),
            Draft = meta.Draft,
            ReadingMinutes = MarkdownRenderer.ReadingMinutes(body),
            Html = markdown.Render(body, media),
        };
    }

    public Page ParsePage(string slug, string text)
    {
        var (meta, body) = FrontMatter.Parse<PageMeta>(text);
        return new Page
        {
            Slug = slug,
            Title = meta.Title ?? slug,
            Summary = meta.Summary ?? "",
            Html = markdown.Render(body, $"/media/pages/{slug}"),
        };
    }

    private static MediaItem? ToMediaItem(object entry, string media) => entry switch
    {
        string src => new MediaItem(Media.Resolve(src, media)!, null),
        IDictionary<object, object> map when map.TryGetValue("src", out var src) && src is string s =>
            new MediaItem(Media.Resolve(s, media)!, map.TryGetValue("caption", out var c) ? c as string : null),
        _ => null,
    };
}
