---
title: Three C# 14 features I now use every day
summary: The field keyword, extension members and null-conditional assignment quietly removed a lot of boilerplate from my code.
date: 2026-09-22
tags: [C#, .NET]
---

C# 14 shipped with .NET 10, and a few of its smaller features have changed how my everyday code looks. Here are the three I reach for most.

## 1. The `field` keyword

Before, adding any logic to a property meant giving up the auto-property and declaring a backing field by hand. Now the accessor can refer to the compiler-generated field directly:

```csharp
public string Name
{
    get;
    set => field = value?.Trim() ?? throw new ArgumentNullException(nameof(value));
}
```

No `_name`, and nothing else in the class can touch the backing field by accident.

## 2. Extension members

Extension *methods* have been around since C# 3. C# 14 adds extension **blocks**, which can also hold properties and static members:

```csharp
public static class StringExtensions
{
    extension(string s)
    {
        public bool IsBlank => string.IsNullOrWhiteSpace(s);

        public string Truncate(int max) =>
            s.Length <= max ? s : string.Concat(s.AsSpan(0, max), "…");
    }
}

if (title.IsBlank) { /* ... */ }
```

`IsBlank` reads like a real property of `string`, which is exactly what I always wanted from extension methods.

## 3. Null-conditional assignment

`?.` now works on the left-hand side of an assignment:

```csharp
// Before
if (customer is not null)
    customer.LastSeen = DateTime.UtcNow;

// After
customer?.LastSeen = DateTime.UtcNow;
```

The right-hand side is only evaluated when `customer` isn't null, so it's safe even when that side is expensive.

---

None of these are headline features, but small wins like these add up across a whole codebase.
