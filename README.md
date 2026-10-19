# Portfolio

My portfolio and blog, built with **ASP.NET Core 10** (Razor Pages). Projects and posts are plain Markdown files with YAML front matter. There's no database and no CMS: add a file, and it shows up on the site.

## Run it

```bash
dotnet run --project src/Portfolio.Web
```

Then open http://localhost:5117. Content reloads automatically whenever a Markdown file changes. Run the tests with `dotnet test`.

## Design

The site uses the `minimal` theme (editorial: serif headlines, warm paper tones, lots of whitespace). Two other sample themes, `terminal` and `studio`, are still in `wwwroot/css/themes/`; to switch, change `Site:Theme` in `src/Portfolio.Web/appsettings.json`.

Each theme is a single file in `wwwroot/css/themes/`, layered over the shared structure in `wwwroot/css/site.css`.

## Adding a project

Create `src/Portfolio.Web/Content/projects/<slug>.md`, and put its images and videos in `wwwroot/media/projects/<slug>/`. The slug becomes the URL, `/projects/<slug>/`.

```markdown
---
title: Fractal Explorer
summary: One sentence for the card and for search results.
date: 2026-05-14
featured: true        # shown on the home page (up to 6)
order: 1              # optional; lower comes first, then newest first
tags: [Graphics, GPU] # each tag gets a page, e.g. /projects/tag/graphics/
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

The `Site` section of `appsettings.json` holds the title, tagline, description, social links, posts per page and `BaseUrl`. `BaseUrl` is the public address (`https://syntacticflow.github.io`); the RSS feed, sitemap and social cards use it.

## Deploying to GitHub Pages

Every push to `main` runs `.github/workflows/pages.yml`, which tests the site, renders it to static HTML and deploys it to GitHub Pages. Drafts are never exported.

One-time setup on GitHub:

1. Rename the repository to `SyntacticFlow.github.io`, so the site is served at the domain root.
2. In **Settings → Pages**, set **Source** to **GitHub Actions**.

To build the static site locally and look at it:

```bash
dotnet publish src/Portfolio.Web -c Release -o out
```

```bash
dotnet out/Portfolio.Web.dll --export _site
```

The export starts the app, follows every link from the home page and writes each page to `_site/<path>/index.html`, with the 404 page as `_site/404.html`. It fails if any internal link is broken. Because GitHub Pages only serves files, every page URL is a plain path ending in `/`: tag filters are `/blog/tag/<tag>/` and older posts are `/blog/page/2/`, never query strings.

## Layout

```
src/Portfolio.Web/
  Content/            Markdown: projects/, posts/, pages/
  Pages/              Razor Pages (home, projects, blog, about, 404)
  Services/           Content loading, Markdown rendering, links, themes, RSS, static export
  wwwroot/css/        site.css + themes/{minimal,terminal,studio}.css
  wwwroot/media/      Images and videos, one folder per project or post
tests/Portfolio.Tests Unit tests for content parsing, plus integration tests for every route
```

The six projects and three posts that ship with the site are **samples**, there to show the layout. Replace them with your own.
