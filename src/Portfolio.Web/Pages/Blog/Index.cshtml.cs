using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Portfolio.Web.Services;

namespace Portfolio.Web.Pages.Blog;

public class IndexModel(ContentStore content, IOptionsSnapshot<SiteOptions> options) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Tag { get; set; }

    // Read by hand: "page" is a reserved route value in Razor Pages, so it can't be model-bound.
    public int CurrentPage { get; private set; } = 1;

    public IReadOnlyList<Post> Posts { get; private set; } = [];
    public IReadOnlyList<string> AllTags { get; private set; } = [];
    public int TotalPages { get; private set; }

    public void OnGet()
    {
        AllTags = content.Posts
            .SelectMany(p => p.Tags)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var matching = string.IsNullOrWhiteSpace(Tag)
            ? content.Posts
            : content.Posts.Where(p => p.Tags.Contains(Tag, StringComparer.OrdinalIgnoreCase)).ToList();

        var size = Math.Max(1, options.Value.PostsPerPage);
        TotalPages = (int)Math.Ceiling(matching.Count / (double)size);
        CurrentPage = int.TryParse(Request.Query["page"], out var requested) ? requested : 1;
        CurrentPage = Math.Clamp(CurrentPage, 1, Math.Max(1, TotalPages));
        Posts = matching.Skip((CurrentPage - 1) * size).Take(size).ToList();
    }
}
