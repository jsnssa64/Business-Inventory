# CLAUDE.md — Inventory-UI

React 19 + TypeScript SPA bundled with Webpack 5. The parent `Business-Inventory/CLAUDE.md` covers the full repo layout and full-stack Docker setup.

## Project at a glance

Single-page app that talks to the Inventory-API over HTTP (Axios) and SignalR. Server state is managed by TanStack React Query, forms by React Hook Form + Zod, styling by Tailwind 4 + DaisyUI 5. Build tooling is custom Webpack (not Vite, not CRA).

## Where to look

- **`ARCHITECTURE.md`** — folder layout, layer responsibilities (api / hooks / components / models), styling structure
- **`BUILD.md`** — Webpack, ESLint, TypeScript, PostCSS, environment variables
- **`CODE_PRACTICES.md`** — conventions and anti-patterns
- **This file** — commands and the hard rules below

## Commands

```bash
npm install
npm start          # Dev server on http://localhost:3000 (hot reload)
npm run build      # Production bundle → dist/
npm run lint       # ESLint (standalone)
npm run test       # Jest (watch mode)
npm run start:prod # Production preview server
```

ESLint runs automatically during every Webpack build, so `npm run lint` is rarely needed during development.

## Hard rules

These are non-negotiable for this project. If a request would violate one, flag it before proceeding.

- **Server state goes through TanStack React Query only.** No `useState`/`useEffect` for data fetched from the API. Hooks live in `src/hooks/`.
- **Forms use React Hook Form + Zod.** Define the Zod schema alongside the form component — don't put validation in handlers.
- **No raw API calls from components.** Components call hooks (`src/hooks/`), hooks call services (`src/api/services/`), services use the Axios instance.
- **DaisyUI classes first, then Tailwind utilities.** For standard UI (buttons, modals, tables), reach for the DaisyUI component class before composing raw Tailwind.
- **Conditional classes use `classnames`**, not template literals.
- **No legacy ESLint config.** The flat config in `eslint.config.js` is canonical; `old-eslint.config.js` is reference only — don't edit it.
