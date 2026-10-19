using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Portfolio.Web.Services;

namespace Portfolio.Tests;

public class SiteTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Theory]
    [InlineData("/")]
    [InlineData("/projects/")]
    [InlineData("/projects/tag/graphics/")]
    [InlineData("/projects/fractal-explorer/")]
    [InlineData("/blog/")]
    [InlineData("/blog/tag/dotnet/")]
    [InlineData("/blog/tag/csharp/")]
    [InlineData("/blog/searchvalues-is-underrated/")]
    [InlineData("/about/")]
    [InlineData("/feed.xml")]
    [InlineData("/sitemap.xml")]
    public async Task Pages_render(string url)
    {
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/does-not-exist")]
    [InlineData("/projects/does-not-exist/")]
    [InlineData("/projects/tag/nope/")]
    [InlineData("/blog/does-not-exist/")]
    [InlineData("/blog/tag/nope/")]
    [InlineData("/blog/page/99/")]
    public async Task Unknown_pages_are_404(string url)
    {
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("wandered off", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Tag_page_lists_only_matching_projects()
    {
        var html = await _client.GetStringAsync("/projects/tag/audio/");

        Assert.Contains("/projects/pixel-synth/", html);
        Assert.DoesNotContain("/projects/tiny-lisp/", html);
    }

    [Fact]
    public async Task Feed_lists_posts_but_never_drafts()
    {
        var feed = await _client.GetStringAsync("/feed.xml");

        Assert.Contains("/blog/searchvalues-is-underrated/", feed);
        Assert.DoesNotContain("draft-webassembly-components", feed);
    }

    [Fact]
    public async Task Pages_use_the_configured_theme()
    {
        var html = await _client.GetStringAsync("/");

        Assert.Contains("data-theme=\"minimal\"", html);
        Assert.DoesNotContain("preview-bar", html);
    }

    [Theory]
    [InlineData("C#", "csharp")]
    [InlineData(".NET", "dotnet")]
    [InlineData("Node.js", "node-js")]
    [InlineData("C++", "cplusplus")]
    [InlineData("Machine Learning", "machine-learning")]
    public void Tag_slugs_are_url_safe(string tag, string slug) => Assert.Equal(slug, Links.TagSlug(tag));

    [Theory]
    [InlineData(null, 1, "/blog/")]
    [InlineData(null, 2, "/blog/page/2/")]
    [InlineData(".NET", 1, "/blog/tag/dotnet/")]
    [InlineData(".NET", 3, "/blog/tag/dotnet/page/3/")]
    public void Blog_page_links(string? tag, int page, string url) => Assert.Equal(url, Links.BlogPage(tag, page));
}
