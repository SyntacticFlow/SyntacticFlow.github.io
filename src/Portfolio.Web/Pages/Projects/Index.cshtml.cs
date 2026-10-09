using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portfolio.Web.Services;

namespace Portfolio.Web.Pages.Projects;

public class IndexModel(ContentStore content) : PageModel
{
    [BindProperty(SupportsGet = true)]
    public string? Tag { get; set; }

    public IReadOnlyList<Project> Projects { get; private set; } = [];
    public IReadOnlyList<(string Tag, int Count)> AllTags { get; private set; } = [];
    public int Total => content.Projects.Count;

    public void OnGet()
    {
        AllTags = content.Projects
            .SelectMany(p => p.Tags)
            .GroupBy(t => t, StringComparer.OrdinalIgnoreCase)
            .Select(g => (g.First(), g.Count()))
            .OrderByDescending(x => x.Item2).ThenBy(x => x.Item1)
            .ToList();

        Projects = string.IsNullOrWhiteSpace(Tag)
            ? content.Projects
            : content.Projects.Where(p => p.Tags.Contains(Tag, StringComparer.OrdinalIgnoreCase)).ToList();
    }
}
