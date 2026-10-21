# Code Practices — Inventory-UI

> Conventions and recipes for working in this codebase. The hard rules in `CLAUDE.md` take precedence; anything here can be deviated from with good reason.

## Conventions

- **Server state through React Query, client state with `useState`/`useReducer`.** If the data came from (or is going to) the API, it's server state. Don't duplicate it into local state.
- **One React Query hook file per resource.** `useInventory.ts`, `useProduct.ts`, etc. Keep all queries and mutations for that resource together.
- **Services are dumb.** A service function takes inputs, returns a typed promise. No React, no caching, no error handling beyond letting Axios throw.
- **Forms: Zod schema lives next to the form component.** Infer the form type from the schema (`z.infer<typeof schema>`); don't define both separately.
- **Conditional classes use `classnames`.** Template literals for class strings are forbidden because they bypass linting and tooling.
- **DaisyUI first, Tailwind second.** For buttons, modals, tables, alerts, etc., reach for the DaisyUI class. Only compose raw Tailwind when DaisyUI doesn't have a matching component.
- **Generic UI in `rootComponents/`, feature UI in `components/`.** When in doubt, start in `components/` and promote later.
- **API response types live in `models/data/`; UI-only types in `models/ui/`.** Don't mix them.
- **Imports: React is implicit** (`"jsx": "react-jsx"`). Don't add `import React from 'react'` unless you actually need the namespace (e.g. `React.FC`, `React.MouseEvent`).

## Recipes

### Adding a new API-backed feature

1. Add response types to `src/models/data/<resource>.ts`.
2. Add service functions to `src/api/services/<resource>.ts` — one function per endpoint, typed in and out.
3. Add a hook file to `src/hooks/use<Resource>.ts` wrapping the service calls in `useQuery` / `useMutation`. Define cache keys here.
4. Build the component in `src/components/<Feature>/` and consume the hook.

### Adding a form

1. Define a Zod schema in the same file (or a sibling file) as the form component.
2. Use `useForm` with `zodResolver(schema)` and infer the form type from the schema.
3. Submit handler calls the relevant mutation hook from `src/hooks/`.

### Adding a reusable UI component

1. If it's domain-agnostic (no business knowledge), add it under `src/rootComponents/`.
2. Style with DaisyUI classes where possible; fall back to Tailwind utilities; only add custom CSS if the pattern repeats.
3. Props should be typed; no `any`.

### Adding a SignalR subscription

1. Use or extend the wrappers in `src/api/signalR/` rather than calling the SignalR client directly from a component.
2. The hub URL comes from `REACT_APP_SIGNALR_HUB_URL` — don't hardcode it.

## Anti-patterns

<!-- Fill these in as you encounter them. Real examples beat abstract advice. -->

- <!-- e.g. Using useEffect to fetch data instead of useQuery. -->
- <!-- e.g. Calling an axios service directly from a component instead of going through a hook. -->
- <!-- e.g. Defining the form type separately from the Zod schema and letting them drift. -->
- <!-- e.g. Writing template-literal class strings: `className={`btn ${active ? 'btn-primary' : ''}`}`. Use classnames(). -->
- <!-- e.g. Reaching for a generic state library when React Query + useState would cover it. -->

## When in doubt

- Match the style of the surrounding code in the same folder.
- Check `ARCHITECTURE.md` for *where* something belongs; `BUILD.md` for build/tooling questions.
- If still unsure, ask.
