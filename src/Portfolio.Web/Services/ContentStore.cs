using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Portfolio.Web.Services;

/// <summary>
/// Holds all site content in memory and reloads it whenever a Markdown file under the
/// content folder changes, so edits show up without restarting the app.
/// </summary>
public sealed class ContentStore : IDisposable
{
    private readonly ContentLoader _loader;
    private readonly string _root;
    private readonly bool _includeDrafts;
    private readonly PhysicalFileProvider? _files;
    private readonly IDisposable? _watch;
    private volatile ContentSnapshot _snapshot;

    public ContentStore(ContentLoader loader, IOptions<SiteOptions> options, IWebHostEnvironment env)
    {
        _loader = loader;
        _root = Path.GetFullPath(Path.Combine(env.ContentRootPath, options.Value.ContentPath));
        _includeDrafts = env.IsDevelopment();
        _snapshot = Directory.Exists(_root) ? loader.Load(_root) : ContentSnapshot.Empty;

        if (Directory.Exists(_root))
        {
            _files = new PhysicalFileProvider(_root);
            _watch = ChangeToken.OnChange(() => _files.Watch("**/*.md"), () => _snapshot = _loader.Load(_root));
        }
    }

    public IReadOnlyList<Project> Projects => _snapshot.Projects;

    /// <summary>Published posts, newest first. Drafts are included only in Development.</summary>
    public IReadOnlyList<Post> Posts => _includeDrafts
        ? _snapshot.Posts
        : _snapshot.Posts.Where(p => !p.Draft).ToList();

    public Project? FindProject(string slug) =>
        Projects.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public Post? FindPost(string slug) =>
        Posts.FirstOrDefault(p => p.Slug.Equals(slug, StringComparison.OrdinalIgnoreCase));

    public Page? FindPage(string slug) => _snapshot.Pages.GetValueOrDefault(slug);

    public void Dispose()
    {
        _watch?.Dispose();
        _files?.Dispose();
    }
}
