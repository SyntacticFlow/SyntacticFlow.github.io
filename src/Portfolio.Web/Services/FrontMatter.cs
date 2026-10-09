using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Portfolio.Web.Services;

/// <summary>Splits a Markdown file into its YAML front matter and body.</summary>
public static partial class FrontMatter
{
    private static readonly IDeserializer Yaml = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .IgnoreUnmatchedProperties()
        .Build();

    [GeneratedRegex(@"\A---[ \t]*\r?\n(?<yaml>.*?)^---[ \t]*(\r?\n|\z)", RegexOptions.Singleline | RegexOptions.Multiline)]
    private static partial Regex Block();

    public static (T Meta, string Body) Parse<T>(string text) where T : new()
    {
        var match = Block().Match(text);
        if (!match.Success)
            return (new T(), text);

        var yaml = match.Groups["yaml"].Value;
        var meta = string.IsNullOrWhiteSpace(yaml) ? new T() : Yaml.Deserialize<T>(yaml) ?? new T();
        return (meta, text[match.Length..]);
    }
}

// YAML shapes. Keys are camelCase in the Markdown files (e.g. `repo:`, `featured:`).

public sealed class ProjectMeta
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public DateTime? Date { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Tech { get; set; }
    public string? Cover { get; set; }

    /// <summary>Each entry is either a path string or a map with <c>src</c> and <c>caption</c>.</summary>
    public List<object>? Gallery { get; set; }

    public string? Video { get; set; }
    public string? Poster { get; set; }
    public string? Repo { get; set; }
    public string? Live { get; set; }
    public bool Featured { get; set; }
    public int Order { get; set; }
}

public sealed class PostMeta
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
    public DateTime? Date { get; set; }
    public List<string>? Tags { get; set; }
    public string? Cover { get; set; }
    public bool Draft { get; set; }
}

public sealed class PageMeta
{
    public string? Title { get; set; }
    public string? Summary { get; set; }
}
