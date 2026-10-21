# Code Practices

> High-level coding preferences and conventions for this project. This is guidance, not law — Claude should use this to inform suggestions and flag deviations, but human judgment wins on edge cases.

## How to use this file

- **For contributors:** Skim this before starting work. It's an overview, not a full style guide.
- **For Claude:** Treat this as project-level priors. When the user asks for code, follow these preferences by default. When the user's request conflicts with something here, mention the conflict briefly and defer to the user.

---

## Project context

<!-- One or two sentences. What is this codebase? What does it do? -->
<!-- e.g. "A TypeScript CLI tool for batch image processing. Used by ~5 internal users." -->

**Stack:** <!-- e.g. TypeScript, Node 20, Vite, Postgres -->
**Scale:** <!-- e.g. small internal tool / production service / library -->

---

## Guiding principles

<!-- 3-5 short principles that capture the spirit. Order matters — most important first. -->

1. **Clarity over cleverness.** If a junior dev would have to read it twice, rewrite it.
2. **Boring is good.** Prefer well-known patterns over novel ones unless there's a real reason.
3. <!-- add your own -->

---

## Language & style

<!-- Things that aren't enforced by linters but matter. Keep it short. -->

- <!-- e.g. Prefer `const` over `let`. Avoid `var` entirely. -->
- <!-- e.g. Function names: verbs (`getUser`, `parseConfig`). Variables: nouns. -->
- <!-- e.g. No single-letter variables except loop counters. -->

## Structure

- <!-- e.g. One concept per file. Files over ~300 lines should be split. -->
- <!-- e.g. Group by feature, not by type (no `controllers/` `models/` `views/` split). -->
- <!-- e.g. Public API lives in `index.ts` at each folder level. -->

## Comments & documentation

- <!-- e.g. Comment the *why*, not the *what*. -->
- <!-- e.g. Every exported function gets a one-line JSDoc. -->
- <!-- e.g. TODOs include a name or ticket: `// TODO(alex): handle null case`. -->

## Error handling

- <!-- e.g. Throw for programmer errors, return Result types for expected failures. -->
- <!-- e.g. Never swallow errors silently. At minimum, log with context. -->

## Testing

- <!-- e.g. Unit tests for pure logic, integration tests for boundaries. -->
- <!-- e.g. Test names describe behaviour: `it("returns null when user is missing")`. -->
- <!-- e.g. No tests against implementation details; test the public contract. -->

## Dependencies

- <!-- e.g. New dependencies need a one-line justification in the PR. -->
- <!-- e.g. Prefer stdlib over a package for trivial utilities. -->

## Git & commits

- <!-- e.g. Conventional commits (`feat:`, `fix:`, `chore:`). -->
- <!-- e.g. One logical change per commit. -->

---

## Anti-patterns (avoid these)

<!-- Specific things you've seen go wrong in this project. More useful than abstract advice. -->

- <!-- e.g. Don't reach for a state management library before there are 3+ shared pieces of state. -->
- <!-- e.g. No `any` in TypeScript without a `// eslint-disable` and a reason. -->

---

## When in doubt

<!-- Optional: who to ask, where to look, default behaviour for ambiguous cases. -->

- Match the style of the surrounding code.
- If still unsure, ask.