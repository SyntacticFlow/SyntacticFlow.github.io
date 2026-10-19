using System.Text;

namespace Portfolio.Web.Services;

/// <summary>
/// Site URLs. Every page URL ends in a slash and carries no query string, so the
/// static export can write each one as <c>{path}/index.html</c>.
/// </summary>
public static class Links
{
    public const string Projects = "/projects/";
    public const string Blog = "/blog/";
    public const string About = "/about/";

    public static string Project(string slug) => $"/projects/{slug}/";
    public static string Post(string slug) => $"/blog/{slug}/";
    public static string ProjectTag(string tag) => $"/projects/tag/{TagSlug(tag)}/";

    public static string BlogPage(string? tag, int page)
    {
        var root = tag is null ? Blog : $"/blog/tag/{TagSlug(tag)}/";
        return page <= 1 ? root : $"{root}page/{page}/";
    }

    /// <summary>"C#" becomes "csharp", ".NET" becomes "dotnet", "Node.js" becomes "node-js".</summary>
    public static string TagSlug(string tag)
    {
        var slug = new StringBuilder();
        var text = tag.Trim().ToLowerInvariant();
        if (text.StartsWith('.')) slug.Append("dot");

        foreach (var c in text.TrimStart('.'))
        {
            if (char.IsAsciiLetterOrDigit(c)) slug.Append(c);
            else if (c == '#') slug.Append("sharp");
            else if (c == '+') slug.Append("plus");
            else if (slug.Length > 0 && slug[^1] != '-') slug.Append('-');
        }
        return slug.ToString().Trim('-');
    }

    public static bool TagMatches(string tag, string slug) =>
        TagSlug(tag).Equals(slug, StringComparison.OrdinalIgnoreCase);
}
