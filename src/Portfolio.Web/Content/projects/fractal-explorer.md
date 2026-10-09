---
title: Fractal Explorer
summary: A GPU-accelerated Mandelbrot and Julia set explorer with smooth deep zooms and video export.
date: 2026-05-14
featured: true
order: 1
tags: [Graphics, GPU, Math]
tech: [C#, .NET 10, Silk.NET, GLSL]
cover: cover.jpg
video: zoom.mp4
poster: cover.jpg
repo: https://github.com/SyntacticFlow
gallery:
  - src: overview.jpg
    caption: The whole set at 1× zoom
  - src: seahorse-valley.jpg
    caption: Seahorse Valley
  - src: deep-zoom.jpg
    caption: Spirals near the end of the zoom
---

> **Sample project.** Replace this file with one of your own. See the README for the front-matter fields.

Fractal Explorer renders the Mandelbrot set in a fragment shader, so panning and zooming stay at 60 fps even on a laptop GPU. When you find a spot you like, it records a keyframed zoom and exports it as an MP4, like the clip above.

## How it works

Each pixel runs the classic escape-time loop on the GPU:

```glsl
vec2 z = vec2(0.0);
int i = 0;
for (; i < maxIter && dot(z, z) < 4.0; i++)
    z = vec2(z.x * z.x - z.y * z.y, 2.0 * z.x * z.y) + c;
```

Colouring uses the *normalised iteration count*, which removes the banding you get from integer iteration counts.

## What I learned

- Single-precision floats run out at around 10⁵× zoom. Past that it switches to perturbation theory, calculating one reference orbit in double precision on the CPU.
- Exporting video is mostly a plumbing problem: render to an off-screen framebuffer, read back the pixels, and pipe raw frames into `ffmpeg`.
