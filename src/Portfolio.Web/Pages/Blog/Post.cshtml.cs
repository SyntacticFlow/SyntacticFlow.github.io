using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Portfolio.Web.Services;

namespace Portfolio.Web.Pages.Blog;

public class PostModel(ContentStore content) : PageModel
{
    public Post Post { get; private set; } = null!;
    public Post? Newer { get; private set; }
    public Post? Older { get; private set; }

    public IActionResult OnGet(string slug)
    {
        if (content.FindPost(slug) is not { } post)
            return NotFound();

        Post = post;
        var all = content.Posts; // newest first
        var index = all.ToList().IndexOf(post);
        Newer = index > 0 ? all[index - 1] : null;
        Older = index < all.Count - 1 ? all[index + 1] : null;
        return Page();
    }
}
