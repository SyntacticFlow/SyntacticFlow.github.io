---
title: Home Lab Dashboard
summary: A real-time dashboard for my home server, sensors and smart plugs, built on Blazor and MQTT.
date: 2026-02-09
featured: true
order: 4
tags: [Web, IoT]
tech: [ASP.NET Core, Blazor, MQTT, SQLite]
cover: cover.svg
repo: https://github.com/SyntacticFlow
gallery:
  - src: cover.svg
    caption: Light mode
  - src: dark-mode.svg
    caption: Dark mode
---

> **Sample project.** Replace this file with one of your own.

Every device in the house publishes to an MQTT broker. A background service subscribes, stores readings in SQLite, and pushes live updates to the Blazor UI over SignalR.

## Highlights

- Sub-second updates without polling
- Energy history with downsampling, so a year of readings still charts instantly
- Automations written as small C# scripts that hot-reload
