using Portfolio.Web.Services;

// `--export <dir>` renders the site to static files instead of serving it. It always runs
// as Production, so drafts are never exported.
var exportTo = args is ["--export", var dir, ..] ? Path.GetFullPath(dir) : null;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    EnvironmentName = exportTo is null ? null : Environments.Production,
    ContentRootPath = exportTo is null ? null : AppContext.BaseDirectory,
});

builder.Services.Configure<SiteOptions>(builder.Configuration.GetSection(SiteOptions.Section));
builder.Services.AddSingleton<MarkdownRenderer>();
builder.Services.AddSingleton<ContentLoader>();
builder.Services.AddSingleton<ContentStore>();
builder.Services.AddRazorPages(options =>
{
    // Tags and page numbers live in the path so every URL maps to a static file.
    options.Conventions.AddPageRoute("/Projects/Index", "projects/tag/{tag}");
    options.Conventions.AddPageRoute("/Blog/Index", "blog/page/{number:int}");
    options.Conventions.AddPageRoute("/Blog/Index", "blog/tag/{tag}");
    options.Conventions.AddPageRoute("/Blog/Index", "blog/tag/{tag}/page/{number:int}");
});
builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

// Load content at startup so a broken content folder shows up in the logs immediately.
app.Services.GetRequiredService<ContentStore>();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found");
if (exportTo is null) app.UseHttpsRedirection();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapFeeds();

if (exportTo is not null)
    return await StaticExport.RunAsync(app, exportTo);

app.Run();
return 0;

public partial class Program;
