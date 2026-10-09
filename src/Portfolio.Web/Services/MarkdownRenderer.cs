using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Portfolio.Web.Services;

public sealed class MarkdownRenderer
{
    private readonly MarkdownPipeline _pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    /// <summary>
    /// Renders Markdown to HTML. Relative image paths (e.g. <c>![](diagram.png)</c>) are
    /// resolved against <paramref name="mediaBase"/>, so content can sit next to its media folder.
    /// </summary>
    public string Render(string markdown, string? mediaBase = null)
    {
        var document = Markdown.Parse(markdown, _pipeline);

        foreach (var link in document.Descendants<LinkInline>())
        {
            if (!link.IsImage) continue;
            if (mediaBase is not null && link.Url is { } url && Media.IsRelative(url))
                link.Url = $"{mediaBase}/{url}";
            link.GetAttributes().AddPropertyIfNotExist("loading", "lazy");
        }

        using var writer = new StringWriter();
        var renderer = new HtmlRenderer(writer);
        _pipeline.Setup(renderer);
        renderer.Render(document);
        writer.Flush();
        return writer.ToString();
    }

    public static int ReadingMinutes(string markdown)
    {
        var words = markdown.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries).Length;
        return Math.Max(1, (int)Math.Round(words / 220.0));
    }
}
