# CLAUDE.md — Inventory-UI

React 19 + TypeScript SPA bundled with Webpack 5.

## Commands

```bash
npm install
npm start          # Dev server on http://localhost:3000 (hot reload)
npm run build      # Production bundle → dist/
npm run lint       # ESLint (standalone)
npm run test       # Jest (watch mode)
npm run start:prod # Production preview server
```

## Source Layout

```
src/
├── api/
│   ├── axios/          # Axios instance config
│   ├── services/       # Per-resource service functions (inventory, product, role, user)
│   └── signalR/        # SignalR component wrappers (general + specific)
├── components/         # Feature components (Inventory, Login, Navigation, Profile, etc.)
├── hooks/              # React Query hooks — one file per resource (useInventory, useProduct, etc.)
├── models/
│   ├── data/           # API response shapes
│   └── ui/             # UI-only types (form models, permission types)
├── rootComponents/     # Generic reusable UI (DataTable, Modal, Tile, Icon, DropDown)
├── style/              # CSS split into base / components / utilities layers
│   ├── index.css       # Entry stylesheet
│   ├── base.css
│   ├── components.css
│   └── utility.css
├── App.tsx
└── index.tsx           # Webpack entry point
```

## Webpack

Config is in `webpack.config.js`. Key details:

- **Entry:** `src/index.tsx`
- **Output:** `dist/[name].[contenthash].js` (cleaned each build)
- **Loaders:**
  - `.tsx?` → `ts-loader`
  - `.js|.jsx` → `babel-loader` (`@babel/preset-env` + `@babel/preset-react`)
  - `.css` → `style-loader` → `css-loader` → `postcss-loader`
  - images → `asset/resource`
- **Plugins:** `HtmlWebpackPlugin`, `ESLintPlugin`, `DotenvWebpackPlugin`
- **Dev server:** `historyApiFallback: true` (all routes fall back to `index.html`)

ESLint runs automatically during every webpack build via `eslint-webpack-plugin` (extensions: `ts`, `tsx`, `js`, `jsx`). You do not need to run `npm run lint` separately during development.

## ESLint

- **Version:** ESLint 9.24.0
- **Config format:** Flat config (`eslint.config.js` exports an array) — ESLint 9 style, not the legacy `.eslintrc` format
- **Plugins in use:** `eslint-plugin-react` (`@eslint/js` recommended base + react rules)
- **`@tanstack/eslint-plugin-query`** is installed in devDependencies — not yet wired into the config

The old legacy config is preserved as `old-eslint.config.js` for reference.

## TypeScript

`tsconfig.json` key settings:

- `"strict": true`
- `"jsx": "react-jsx"` — no need to import React in every file
- `"moduleResolution": "bundler"`
- `"target"` and `"module"`: ES6

No `@typescript-eslint` parser is configured — TypeScript errors surface through `ts-loader` at build time, not ESLint.

## Styling

Tailwind CSS 4 + DaisyUI 5, processed through PostCSS (`postcss.config.js`). Custom CSS is layered as:

- `style/base.css` — resets and base element styles
- `style/components.css` — component-level classes (`@import`s from `style/components/`)
- `style/utility.css` — utility overrides (`@import`s from `style/utilities/`)
- `style/index.css` — imports all three layers + Tailwind directives

## Environment Variables

Injected at build time by `dotenv-webpack`. Create the relevant file before starting:

- `.env.development` — used by `npm start`
- `.env.production` — used by `npm run build` / `npm run start:prod`

Key variables:

```env
REACT_APP_API_URL=http://localhost:3001
REACT_APP_SIGNALR_HUB_URL=http://localhost:5050/hub
```

## Conventions

- Server state is managed exclusively through TanStack React Query v5. Hooks live in `src/hooks/` and wrap `src/api/services/` calls.
- Forms use React Hook Form v7 + Zod v3 for validation. Define the Zod schema alongside the form component.
- Use `classnames` for conditional class merging, not template literals.
- DaisyUI component classes are preferred over writing raw Tailwind for standard UI elements (buttons, modals, tables).
