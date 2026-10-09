using Microsoft.Extensions.Options;

namespace Portfolio.Web.Services;

public sealed record Theme(string Id, string Name, string Description, string FontsUrl);

/// <summary>
/// The three sample design directions. Each is a stylesheet in wwwroot/css/themes/
/// layered over the shared structure in site.css.
/// </summary>
public static class Themes
{
    public const string CookieName = "theme";

    public static readonly IReadOnlyList<Theme> All =
    [
        new("minimal", "Minimal", "Editorial: serif headlines, quiet colour, lots of whitespace.",
            "https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&family=Newsreader:ital,opsz,wght@0,6..72,400;0,6..72,600;1,6..72,400&family=JetBrains+Mono:wght@400;500&display=swap"),
        new("terminal", "Terminal", "Dark, monospaced, and unapologetically for developers.",
            "https://fonts.googleapis.com/css2?family=JetBrains+Mono:ital,wght@0,400;0,500;0,700;1,400&display=swap"),
        new("studio", "Studio", "Bold display type, gradient accents and big imagery.",
            "https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@500;600;700&family=Inter:wght@400;500;600&family=JetBrains+Mono:wght@400;500&display=swap"),
    ];

    public static Theme? Find(string? id) =>
        All.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase));

    /// <summary>Query string, then cookie (both only while previewing), then the configured theme.</summary>
    public static Theme Resolve(HttpContext context, SiteOptions options)
    {
        if (options.AllowThemePreview)
        {
            if (Find(context.Request.Query["theme"]) is { } fromQuery) return fromQuery;
            if (Find(context.Request.Cookies[CookieName]) is { } fromCookie) return fromCookie;
        }
        return Find(options.Theme) ?? All[0];
    }

    /// <summary>Remembers a <c>?theme=</c> choice so it sticks while browsing the preview.</summary>
    public static IApplicationBuilder UseThemePreview(this IApplicationBuilder app) => app.Use(async (context, next) =>
    {
        var options = context.RequestServices.GetRequiredService<IOptionsSnapshot<SiteOptions>>().Value;
        if (options.AllowThemePreview && Find(context.Request.Query["theme"]) is { } theme)
        {
            context.Response.Cookies.Append(CookieName, theme.Id, new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                SameSite = SameSiteMode.Lax,
                MaxAge = TimeSpan.FromDays(30),
            });
        }
        await next();
    });
}
