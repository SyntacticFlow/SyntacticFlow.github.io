using Microsoft.Extensions.Logging.Abstractions;
using Portfolio.Web.Services;

namespace Portfolio.Tests;

public class ContentTests
{
    private readonly ContentLoader _loader = new(new MarkdownRenderer(), NullLogger<ContentLoader>.Instance);

    [Fact]
    public void FrontMatter_is_split_from_body()
    {
        var (meta, body) = FrontMatter.Parse<PostMeta>("---\ntitle: Hello\ntags: [a, b]\ndraft: true\n---\n# Body\n");

        Assert.Equal("Hello", meta.Title);
        Assert.Equal(["a", "b"], meta.Tags);
        Assert.True(meta.Draft);
        Assert.Equal("# Body\n", body);
    }

    [Theory]
    [InlineData("# No front matter")]
    [InlineData("---\n---\n# Empty front matter")]
    public void Missing_or_empty_front_matter_gives_defaults(string text)
    {
        var (meta, body) = FrontMatter.Parse<PostMeta>(text);

        Assert.Null(meta.Title);
        Assert.EndsWith("front matter", body);
    }

    [Fact]
    public void Project_media_paths_resolve_to_its_media_folder()
    {
        var project = _loader.ParseProject("demo", """
            ---
            title: Demo
            cover: cover.png
            gallery:
              - one.png
              - src: /shared/two.png
                caption: Two
              - src: https://cdn.example.com/three.png
            ---
            ![diagram](diagram.svg) and ![abs](/x.png)
            """);

        Assert.Equal("/media/projects/demo/cover.png", project.Cover);
        Assert.Equal(
            [new MediaItem("/media/projects/demo/one.png", null), new MediaItem("/shared/two.png", "Two"), new MediaItem("https://cdn.example.com/three.png", null)],
            project.Gallery);
        Assert.Contains("src=\"/media/projects/demo/diagram.svg\"", project.Html);
        Assert.Contains("src=\"/x.png\"", project.Html);
        Assert.Contains("loading=\"lazy\"", project.Html);
    }

    [Theory]
    [InlineData("https://www.youtube.com/watch?v=dQw4w9WgXcQ", VideoKind.YouTube, "https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ")]
    [InlineData("https://youtu.be/dQw4w9WgXcQ?t=10", VideoKind.YouTube, "https://www.youtube-nocookie.com/embed/dQw4w9WgXcQ")]
    [InlineData("https://vimeo.com/76979871", VideoKind.Vimeo, "https://player.vimeo.com/video/76979871")]
    [InlineData("demo.mp4", VideoKind.File, "/media/projects/p/demo.mp4")]
    [InlineData("https://cdn.example.com/clip.webm", VideoKind.File, "https://cdn.example.com/clip.webm")]
    public void Videos_are_recognised(string input, VideoKind kind, string url)
    {
        Assert.Equal(new VideoEmbed(kind, url), Media.ParseVideo(input, "/media/projects/p"));
    }

    [Fact]
    public void Unknown_video_links_are_ignored() =>
        Assert.Null(Media.ParseVideo("https://example.com/page", "/media"));

    [Fact]
    public void A_broken_file_is_skipped_not_fatal()
    {
        var root = Directory.CreateTempSubdirectory();
        try
        {
            Directory.CreateDirectory(Path.Combine(root.FullName, "posts"));
            File.WriteAllText(Path.Combine(root.FullName, "posts", "good.md"), "---\ntitle: Good\ndate: 2026-01-02\n---\nHi");
            File.WriteAllText(Path.Combine(root.FullName, "posts", "bad.md"), "---\ntitle: [unclosed\n---\nOops");

            var snapshot = _loader.Load(root.FullName);

            var post = Assert.Single(snapshot.Posts);
            Assert.Equal("good", post.Slug);
            Assert.Equal(new DateOnly(2026, 1, 2), post.Date);
        }
        finally
        {
            root.Delete(recursive: true);
        }
    }
}
