# Portfolio

My portfolio and blog, built with **ASP.NET Core 10** (Razor Pages). Projects and posts are plain Markdown files with YAML front matter. There's no database and no CMS: add a file, and it shows up on the site.

## Run it

```bash
dotnet run --project src/Portfolio.Web
```

Then open http://localhost:5117. Content reloads automatically whenever a Markdown file changes. Run the tests with `dotnet test`.

## Choosing a design

The site ships with three sample designs, and a bar at the top of every page switches between them:

| Theme      | Feel |
|------------|------|
| `minimal`  | Editorial: serif headlines, warm paper tones, lots of whitespace |
| `terminal` | Dark and monospaced, with prompts, `[tags]` and a blinking cursor |
| `studio`   | Bold display type, gradient accents, big imagery and soft shadows |

When you've picked one, set it in `src/Portfolio.Web/appsettings.json` and turn the preview bar off:

```json
"Site": {
  "Theme": "studio",
  "AllowThemePreview": false
}
```

Each theme is a single file in `wwwroot/css/themes/`, layered over the shared structure in `wwwroot/css/site.css`.

## Adding a project

Create `src/Portfolio.Web/Content/projects/<slug>.md`, and put its images and videos in `wwwroot/media/projects/<slug>/`. The slug becomes the URL, `/projects/<slug>`.

```markdown
---
title: Fractal Explorer
summary: One sentence for the card and for search results.
date: 2026-05-14
featured: true        # shown on the home page (up to 6)
order: 1              # optional; lower comes first, then newest first
tags: [Graphics, GPU] # used for filtering on /projects
tech: [C#, GLSL]      # shown on the card
cover: cover.jpg
video: zoom.mp4       # or a YouTube / Vimeo link
poster: cover.jpg     # optional thumbnail shown before a local video plays
repo: https://github.com/you/project
live: https://example.com
gallery:
  - screenshot-1.png
  - src: screenshot-2.png
    caption: Optional caption
---

The write-up, in Markdown. ![Inline images](diagram.png) also resolve to the media folder.
```

Bare file names resolve to the project's media folder. Paths starting with `/` and full URLs are used as-is.

### Videos

- **YouTube or Vimeo** (recommended for anything over a few MB): paste the normal watch URL into `video:` and it's embedded privacy-friendly (`youtube-nocookie.com`).
- **Self-hosted** `.mp4` / `.webm`: drop the file into the media folder. Keep it short and compressed (`ffmpeg -i in.mov -crf 28 -preset slow -movflags +faststart out.mp4`). For big files, use Git LFS or blob storage and link to them with a full URL.

## Writing a post

Create `src/Portfolio.Web/Content/posts/<slug>.md`:

```markdown
---
title: SearchValues is the most underrated type in .NET
summary: Shown in the post list and the RSS feed.
date: 2026-07-30
tags: [.NET, Performance]
draft: true   # only visible when running in Development
---
```

Fenced code blocks are syntax-highlighted. Posts are also published at `/feed.xml` (RSS) and `/sitemap.xml`.

The About page lives in `Content/pages/about.md`.

## Site settings

The `Site` section of `appsettings.json` holds the title, tagline, description, social links, posts per page and `BaseUrl`. Set `BaseUrl` to your real domain before deploying; the RSS feed and sitemap use it.

## Deploying

```bash
dotnet publish src/Portfolio.Web -c Release -o out
```

`out/` is self-contained apart from the .NET 10 runtime, and runs anywhere: Azure App Service, a Linux VM behind nginx, or a container (`dotnet publish src/Portfolio.Web -c Release /t:PublishContainer`). The `Content/` folder is published alongside the app.

Static files are fingerprinted and pre-compressed at publish time, so always deploy a **published** build rather than running `dotnet run` in production.

## Layout

```
src/Portfolio.Web/
  Content/            Markdown: projects/, posts/, pages/
  Pages/              Razor Pages (home, projects, blog, about, 404)
  Services/           Content loading, Markdown rendering, media helpers, themes, RSS
  wwwroot/css/        site.css + themes/{minimal,terminal,studio}.css
  wwwroot/media/      Images and videos, one folder per project or post
tests/Portfolio.Tests Unit tests for content parsing, plus integration tests for every route
```

The six projects and three posts that ship with the site are **samples**, there to show the layout. Replace them with your own.
