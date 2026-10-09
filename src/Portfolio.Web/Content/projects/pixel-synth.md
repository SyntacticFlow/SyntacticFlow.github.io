---
title: Pixel Synth
summary: A browser synthesizer with three oscillators, an ADSR envelope and a live oscilloscope.
date: 2025-04-03
featured: true
order: 5
tags: [Audio, Web]
tech: [TypeScript, Web Audio API, Canvas]
cover: cover.svg
live: https://example.com
---

> **Sample project.** Replace this file with one of your own.

Pixel Synth is built on the Web Audio API: oscillators feed a gain node driven by an ADSR envelope, and an `AnalyserNode` draws the waveform onto a canvas at 60 fps. You can play it with the keyboard (`A`–`L` for the white keys) or with any MIDI controller.
