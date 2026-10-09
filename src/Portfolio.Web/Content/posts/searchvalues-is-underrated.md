---
title: SearchValues is the most underrated type in .NET
summary: If you ever call IndexOfAny in a hot loop, SearchValues<T> lets the runtime pick a vectorised search strategy once instead of on every call.
date: 2026-07-30
tags: [.NET, Performance]
---

`SearchValues<T>` arrived in .NET 8 and got a big upgrade in .NET 9, but I rarely see it in the wild. The idea is simple: you tell the runtime up front which values you're searching for, and it precomputes the fastest way to find them.

## Characters

```csharp
using System.Buffers;

static readonly SearchValues<char> Separators = SearchValues.Create(",;|\t");

static int NextField(ReadOnlySpan<char> line) => line.IndexOfAny(Separators);
```

Depending on the set, the runtime might choose a bitmap, a vectorised range check, or a packed SIMD lookup. Either way, that decision happens **once**, when the field is initialised, rather than on every call.

## Whole strings (.NET 9+)

The bigger win is searching for several substrings at once:

```csharp
static readonly SearchValues<string> LogLevels =
    SearchValues.Create(["error", "warn", "fatal"], StringComparison.OrdinalIgnoreCase);

bool interesting = line.AsSpan().ContainsAny(LogLevels);
```

Under the hood this uses algorithms such as Aho–Corasick and Teddy, the same family of techniques that fast grep tools rely on.

## When to use it

- The set of values is known ahead of time (store it in a `static readonly` field)
- The search runs often: parsers, tokenisers, log processing, sanitising input

If either isn't true, plain `IndexOfAny` is fine.
