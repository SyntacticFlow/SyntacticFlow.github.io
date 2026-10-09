---
title: Tiny Lisp
summary: A Lisp interpreter in about 600 lines of Rust, with macros, tail calls and a REPL.
date: 2025-08-20
featured: true
order: 3
tags: [Languages, Interpreters]
tech: [Rust]
cover: cover.svg
repo: https://github.com/SyntacticFlow
gallery:
  - src: macros.svg
    caption: Quasiquote-based macros
---

> **Sample project.** Replace this file with one of your own.

Writing an interpreter is the fastest way I know to understand how languages work. Tiny Lisp has:

1. A hand-written reader (tokeniser + recursive-descent parser)
2. Environments as linked frames with `Rc<RefCell<…>>`
3. `defmacro` with quasiquote and unquote-splicing
4. Proper tail calls through a trampoline, so `(loop 1000000)` doesn't overflow the stack

```rust
fn eval(expr: &Value, env: &Env) -> Result<Value, Error> {
    match expr {
        Value::Symbol(s) => env.lookup(s),
        Value::List(items) if !items.is_empty() => apply(items, env),
        other => Ok(other.clone()),
    }
}
```
