---
title: Weekend Ray Tracer
summary: A from-scratch ray tracer with reflections, soft shadows and a checkerboard floor that every ray tracer is legally required to have.
date: 2025-11-02
featured: true
order: 2
tags: [Graphics, Math]
tech: [C#, SIMD, ImageSharp]
cover: cover.jpg
repo: https://github.com/SyntacticFlow
gallery:
  - src: sunset.jpg
    caption: Low sun, warm sky
  - src: mirrors.jpg
    caption: Six spheres, three of them mirrors
---

> **Sample project.** Replace this file with one of your own.

A ray tracer is the best weekend project I know: about 300 lines of code and you get images like the one above.

## Features

- Spheres and planes, with Phong shading and hard shadows
- Recursive reflections with a configurable bounce depth
- Distance fog and supersampled anti-aliasing
- `Vector3` everywhere, so .NET's SIMD support does most of the speed-up work

## Next up

Path tracing with proper global illumination, and a BVH so it can handle meshes with more than a dozen primitives.
