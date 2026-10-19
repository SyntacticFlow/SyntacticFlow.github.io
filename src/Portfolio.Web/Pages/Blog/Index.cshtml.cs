using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Portfolio.Web.Services;

namespace Portfolio.Web.Pages.Blog;

public class IndexModel(ContentStore content, IOptionsSnapshot<SiteOptions> options) : PageModel
{
    /// <summary>The tag's URL slug, from <c>/blog/tag/{tag}</c>.</summary>
    [BindProperty(SupportsGet = true)]
    public string? Tag { get; set; }

    /// <summary>From <c>/blog/page/{number}</c>. Named so it can't clash with Razor Pages' reserved "page" route value.</summary>
    [BindProperty(SupportsGet = true)]
    public int Number { get; set; } = 1;

    /// <summary>The tag as written in front matter, once <see cref="Tag"/> has matched one.</summary>
    public string? TagName { get; private set; }

    public int CurrentPage => Number;
    public IReadOnlyList<Post> Posts { get; private set; } = [];
    public IReadOnlyList<string> AllTags { get; private set; } = [];
    public int TotalPages { get; private set; }

    public IActionResult OnGet()
    {
        AllTags = content.Posts
            .SelectMany(p => p.Tags)
            .DistinctBy(Links.TagSlug)
            .Order(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var matching = content.Posts;
        if (!string.IsNullOrWhiteSpace(Tag))
        {
            TagName = AllTags.FirstOrDefault(t => Links.TagMatches(t, Tag));
            if (TagName is null)
                return NotFound();
            matching = content.Posts.Where(p => p.Tags.Any(t => Links.TagMatches(t, Tag))).ToList();
        }

        var size = Math.Max(1, options.Value.PostsPerPage);
        TotalPages = Math.Max(1, (int)Math.Ceiling(matching.Count / (double)size));
        if (Number < 1 || Number > TotalPages)
            return NotFound();

        Posts = matching.Skip((Number - 1) * size).Take(size).ToList();
        return Page();
    }
}
