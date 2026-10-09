using Portfolio.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<SiteOptions>(builder.Configuration.GetSection(SiteOptions.Section));
builder.Services.AddSingleton<MarkdownRenderer>();
builder.Services.AddSingleton<ContentLoader>();
builder.Services.AddSingleton<ContentStore>();
builder.Services.AddRazorPages();
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
app.UseHttpsRedirection();
app.UseThemePreview();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();
app.MapFeeds();

app.Run();

public partial class Program;
