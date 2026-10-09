using System.Xml.Linq;
using Microsoft.Extensions.Options;

namespace Portfolio.Web.Services;

public static class FeedEndpoints
{
    public static IEndpointRouteBuilder MapFeeds(this IEndpointRouteBuilder app)
    {
        app.MapGet("/feed.xml", (ContentStore content, IOptionsSnapshot<SiteOptions> options) =>
        {
            var site = options.Value;
            var baseUrl = site.BaseUrl.TrimEnd('/');

            var channel = new XElement("channel",
                new XElement("title", site.Title),
                new XElement("link", baseUrl),
                new XElement("description", site.Description),
                content.Posts.Where(p => !p.Draft).Take(20).Select(post => new XElement("item",
                    new XElement("title", post.Title),
                    new XElement("link", $"{baseUrl}/blog/{post.Slug}"),
                    new XElement("guid", $"{baseUrl}/blog/{post.Slug}"),
                    new XElement("pubDate", post.Date.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc).ToString("R")),
                    new XElement("description", post.Summary),
                    post.Tags.Select(t => new XElement("category", t)))));

            var xml = new XDocument(new XDeclaration("1.0", "utf-8", null), new XElement("rss", new XAttribute("version", "2.0"), channel));
            return Results.Text(xml.Declaration + "\n" + xml.Root, "application/rss+xml; charset=utf-8");
        });

        app.MapGet("/sitemap.xml", (ContentStore content, IOptionsSnapshot<SiteOptions> options) =>
        {
            XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var baseUrl = options.Value.BaseUrl.TrimEnd('/');

            var paths = new[] { "/", "/projects", "/blog", "/about" }
                .Concat(content.Projects.Select(p => $"/projects/{p.Slug}"))
                .Concat(content.Posts.Where(p => !p.Draft).Select(p => $"/blog/{p.Slug}"));

            var urlset = new XElement(ns + "urlset", paths.Select(p => new XElement(ns + "url", new XElement(ns + "loc", baseUrl + p))));
            return Results.Text(new XDeclaration("1.0", "utf-8", null) + "\n" + urlset, "application/xml; charset=utf-8");
        });

        return app;
    }
}
