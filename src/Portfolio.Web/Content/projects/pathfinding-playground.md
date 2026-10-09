---
title: Pathfinding Playground
summary: Draw walls, drop a start and a goal, and watch A*, Dijkstra and BFS race to the finish.
date: 2024-10-12
featured: true
order: 6
tags: [Algorithms, Web]
tech: [TypeScript, Canvas]
cover: cover.svg
live: https://example.com
gallery:
  - src: dijkstra.svg
    caption: Dijkstra explores in every direction
  - src: cover.svg
    caption: A* on the same maze heads straight for the goal
  - src: astar-dense.svg
    caption: A* on another maze
---

> **Sample project.** Replace this file with one of your own.

The colours show the order cells were visited, and comparing algorithms on the same maze makes the difference obvious: Dijkstra floods outward, while A*'s heuristic pulls it toward the goal.

| Algorithm | Optimal? | Uses heuristic |
|-----------|----------|----------------|
| BFS       | Yes (unweighted) | No |
| Dijkstra  | Yes      | No             |
| A*        | Yes (admissible h) | Yes      |
