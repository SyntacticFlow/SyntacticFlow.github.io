using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portfolio.Web.Services;

namespace Portfolio.Web.Pages.Projects;

public class DetailModel(ContentStore content) : PageModel
{
    public Project Project { get; private set; } = null!;
    public Project? Previous { get; private set; }
    public Project? Next { get; private set; }

    public IActionResult OnGet(string slug)
    {
        if (content.FindProject(slug) is not { } project)
            return NotFound();

        Project = project;
        var all = content.Projects;
        var index = all.ToList().IndexOf(project);
        Previous = index > 0 ? all[index - 1] : null;
        Next = index < all.Count - 1 ? all[index + 1] : null;
        return Page();
    }
}
