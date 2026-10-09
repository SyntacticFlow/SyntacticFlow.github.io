using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Portfolio.Tests;

public class SiteTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    // No cookie jar, so a theme picked in one test can't leak into another.
    private readonly HttpClient _client = factory.CreateClient(new() { HandleCookies = false });

    [Theory]
    [InlineData("/")]
    [InlineData("/projects")]
    [InlineData("/projects?tag=graphics")]
    [InlineData("/projects/fractal-explorer")]
    [InlineData("/blog")]
    [InlineData("/blog?page=2")]
    [InlineData("/blog/searchvalues-is-underrated")]
    [InlineData("/about")]
    [InlineData("/feed.xml")]
    [InlineData("/sitemap.xml")]
    public async Task Pages_render(string url)
    {
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("/does-not-exist")]
    [InlineData("/projects/does-not-exist")]
    [InlineData("/blog/does-not-exist")]
    public async Task Unknown_pages_are_404(string url)
    {
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("wandered off", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Feed_lists_posts_but_never_drafts()
    {
        var feed = await _client.GetStringAsync("/feed.xml");

        Assert.Contains("/blog/searchvalues-is-underrated", feed);
        Assert.DoesNotContain("draft-webassembly-components", feed);
    }

    [Fact]
    public async Task Theme_preview_switches_theme_and_remembers_it()
    {
        var response = await _client.GetAsync("/?theme=terminal");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Contains("data-theme=\"terminal\"", html);
        Assert.Contains(response.Headers.GetValues("Set-Cookie"), c => c.StartsWith("theme=terminal"));
    }

    [Fact]
    public async Task Unknown_theme_falls_back_to_configured_one()
    {
        var html = await _client.GetStringAsync("/?theme=nope");
        Assert.Contains("data-theme=\"minimal\"", html);
    }
}
