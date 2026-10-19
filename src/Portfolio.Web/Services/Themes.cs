namespace Portfolio.Web.Services;

public sealed record Theme(string Id, string Name, string Description, string FontsUrl);

/// <summary>
/// The three sample design directions. Each is a stylesheet in wwwroot/css/themes/
/// layered over the shared structure in site.css.
/// </summary>
public static class Themes
{
    public static readonly IReadOnlyList<Theme> All =
    [
        new("minimal", "Minimal", "Editorial: serif headlines, quiet colour, lots of whitespace.",
            "https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600&family=Newsreader:ital,opsz,wght@0,6..72,400;0,6..72,600;1,6..72,400&family=JetBrains+Mono:wght@400;500&display=swap"),
        new("terminal", "Terminal", "Dark, monospaced, and unapologetically for developers.",
            "https://fonts.googleapis.com/css2?family=JetBrains+Mono:ital,wght@0,400;0,500;0,700;1,400&display=swap"),
        new("studio", "Studio", "Bold display type, gradient accents and big imagery.",
            "https://fonts.googleapis.com/css2?family=Space+Grotesk:wght@500;600;700&family=Inter:wght@400;500;600&family=JetBrains+Mono:wght@400;500&display=swap"),
    ];

    public static Theme Find(string? id) =>
        All.FirstOrDefault(t => t.Id.Equals(id, StringComparison.OrdinalIgnoreCase)) ?? All[0];
}
