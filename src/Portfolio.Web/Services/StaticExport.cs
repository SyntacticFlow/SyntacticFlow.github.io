using System.Net;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;

namespace Portfolio.Web.Services;

/// <summary>
/// Renders the whole site to static files for GitHub Pages. Starts the app on a loopback
/// port, follows every same-site link from the home page (pages, stylesheets, scripts,
/// media), and writes each response to disk. Page URLs become <c>{path}/index.html</c>
/// and the 404 page becomes <c>404.html</c>.
/// </summary>
public static partial class StaticExport
{
    private const string NotFoundPath = "/not-found";

    [GeneratedRegex("""(?:href|src|poster)\s*=\s*"(?<url>[^"]+)"|url\(\s*['"]?(?<url>[^'")\s]+)""", RegexOptions.IgnoreCase)]
    private static partial Regex Reference();

    public static async Task<int> RunAsync(WebApplication app, string outputDir)
    {
        app.Urls.Clear();
        app.Urls.Add("http://127.0.0.1:0");
        await app.StartAsync();

        var address = app.Services.GetRequiredService<IServer>().Features.Get<IServerAddressesFeature>()!.Addresses.First();
        var origin = new Uri(address);
        using var http = new HttpClient { BaseAddress = origin };

        if (Directory.Exists(outputDir)) Directory.Delete(outputDir, recursive: true);
        Directory.CreateDirectory(outputDir);

        string[] seeds = ["/", "/feed.xml", "/sitemap.xml", "/favicon.svg", NotFoundPath];
        var queue = new Queue<string>(seeds);
        var seen = new HashSet<string>(seeds, StringComparer.Ordinal);
        var broken = new List<string>();

        while (queue.TryDequeue(out var path))
        {
            using var response = await http.GetAsync(path);
            var isNotFoundPage = path == NotFoundPath;
            if (!response.IsSuccessStatusCode && !(isNotFoundPage && response.StatusCode == HttpStatusCode.NotFound))
            {
                broken.Add($"{(int)response.StatusCode} {path}");
                continue;
            }

            var body = await response.Content.ReadAsByteArrayAsync();
            var mediaType = response.Content.Headers.ContentType?.MediaType;
            var file = isNotFoundPage ? "404.html" : FileFor(path, mediaType == "text/html");
            var target = Path.Combine(outputDir, file);
            Directory.CreateDirectory(Path.GetDirectoryName(target)!);
            await File.WriteAllBytesAsync(target, body);

            if (mediaType is not ("text/html" or "text/css")) continue;

            var text = System.Text.Encoding.UTF8.GetString(body);
            foreach (Match match in Reference().Matches(text))
            {
                if (SameSitePath(origin, path, WebUtility.HtmlDecode(match.Groups["url"].Value)) is { } next && seen.Add(next))
                    queue.Enqueue(next);
            }
        }

        await app.StopAsync();

        Console.WriteLine($"Exported {seen.Count - broken.Count} files to {outputDir}");
        foreach (var line in broken) Console.Error.WriteLine($"Broken link: {line}");
        return broken.Count == 0 ? 0 : 1;
    }

    /// <summary>The path of a link on this site, without query or fragment; null for anything external.</summary>
    private static string? SameSitePath(Uri origin, string fromPath, string url)
    {
        if (!Uri.TryCreate(new Uri(origin, fromPath), url, out var resolved)) return null;
        if (resolved.Scheme != origin.Scheme || resolved.Authority != origin.Authority) return null;
        return resolved.AbsolutePath;
    }

    private static string FileFor(string path, bool isHtml)
    {
        var relative = Uri.UnescapeDataString(path).Trim('/');
        return isHtml && !relative.EndsWith(".html", StringComparison.OrdinalIgnoreCase)
            ? Path.Combine(relative, "index.html")
            : relative;
    }
}
