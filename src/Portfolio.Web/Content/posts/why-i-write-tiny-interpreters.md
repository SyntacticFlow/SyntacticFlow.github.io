---
title: Why I keep writing tiny interpreters
summary: Every language I've learned properly, I learned by building a small version of it first.
date: 2026-03-18
tags: [Languages, Learning]
---

Every few months I write another tiny interpreter: a Lisp, a stack machine, a regex engine, a JSON parser that's more forgiving than it should be. None of them are useful. All of them were worth it.

## You find out what's actually hard

Reading about closures is one thing. Implementing them, and realising an environment is just a linked list of dictionaries, makes them click in a way no blog post (including this one) can.

## The feedback loop is instant

An interpreter is a small program with a huge input space. Every new feature is a few dozen lines, and you can test it immediately in a REPL you wrote yourself.

> The best way to understand a system is to build a smaller one that works the same way.

## Start here

If you want to try it, pick a Lisp: the syntax is trivial to parse, so you can spend your time on evaluation. Aim for `define`, `lambda`, `if` and arithmetic, then add macros once those work.
