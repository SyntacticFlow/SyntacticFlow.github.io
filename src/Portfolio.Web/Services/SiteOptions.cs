namespace Portfolio.Web.Services;

/// <summary>Bound from the <c>Site</c> section of appsettings.json.</summary>
public sealed class SiteOptions
{
    public const string Section = "Site";

    public string Title { get; set; } = "Portfolio";
    public string Author { get; set; } = "";
    public string Tagline { get; set; } = "";
    public string Description { get; set; } = "";

    /// <summary>Absolute public URL, used for the RSS feed, sitemap and social cards.</summary>
    public string BaseUrl { get; set; } = "https://localhost";

    /// <summary>Folder holding projects/, posts/ and pages/, relative to the content root.</summary>
    public string ContentPath { get; set; } = "Content";

    /// <summary>One of the ids in <see cref="Themes.All"/>.</summary>
    public string Theme { get; set; } = "minimal";

    /// <summary>Shows the theme switcher in the footer and honours <c>?theme=</c>. Turn off once you've picked one.</summary>
    public bool AllowThemePreview { get; set; }

    public int PostsPerPage { get; set; } = 10;

    public List<SocialLink> Social { get; set; } = [];
}

public sealed class SocialLink
{
    public string Name { get; set; } = "";
    public string Url { get; set; } = "";
}
