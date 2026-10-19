using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portfolio.Web.Services;

namespace Portfolio.Web.Pages.Projects;

public class IndexModel(ContentStore content) : PageModel
{
    /// <summary>The tag's URL slug, from <c>/projects/tag/{tag}</c>.</summary>
    [BindProperty(SupportsGet = true)]
    public string? Tag { get; set; }

    /// <summary>The tag as written in front matter, once <see cref="Tag"/> has matched one.</summary>
    public string? TagName { get; private set; }

    public IReadOnlyList<Project> Projects { get; private set; } = [];
    public IReadOnlyList<(string Tag, int Count)> AllTags { get; private set; } = [];
    public int Total => content.Projects.Count;

    public IActionResult OnGet()
    {
        AllTags = content.Projects
            .SelectMany(p => p.Tags)
            .GroupBy(Links.TagSlug)
            .Select(g => (g.First(), g.Count()))
            .OrderByDescending(x => x.Item2).ThenBy(x => x.Item1)
            .ToList();

        if (string.IsNullOrWhiteSpace(Tag))
        {
            Projects = content.Projects;
            return Page();
        }

        TagName = AllTags.Select(t => t.Tag).FirstOrDefault(t => Links.TagMatches(t, Tag));
        if (TagName is null)
            return NotFound();

        Projects = content.Projects.Where(p => p.Tags.Any(t => Links.TagMatches(t, Tag))).ToList();
        return Page();
    }
}
