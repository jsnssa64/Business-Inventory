# Build & Tooling — Inventory-UI

Everything about how the app compiles, lints, and consumes config. If something feels like a "why is the build doing that" question, look here.

## Webpack

Config is in `webpack.config.js`.

- **Entry:** `src/index.tsx`
- **Output:** `dist/[name].[contenthash].js` (cleaned each build)
- **Dev server:** `historyApiFallback: true` — all unknown routes fall back to `index.html` (required for client-side routing)

### Loaders

| Extension | Loader chain |
|---|---|
| `.tsx`, `.ts` | `ts-loader` |
| `.js`, `.jsx` | `babel-loader` (`@babel/preset-env` + `@babel/preset-react`) |
| `.css` | `style-loader` → `css-loader` → `postcss-loader` |
| images | `asset/resource` |

### Plugins

- `HtmlWebpackPlugin` — generates the HTML shell
- `ESLintPlugin` (`eslint-webpack-plugin`) — runs ESLint on every build
- `DotenvWebpackPlugin` — injects env vars at build time

## ESLint

- **Version:** ESLint 9.24.0
- **Config format:** Flat config (`eslint.config.js` exports an array). This is the ESLint 9 style — not the legacy `.eslintrc` format.
- **Plugins in use:** `eslint-plugin-react` on top of `@eslint/js` recommended.
- **Extensions linted:** `ts`, `tsx`, `js`, `jsx`.
- **Auto-runs** during every Webpack build via `eslint-webpack-plugin`.
- `@tanstack/eslint-plugin-query` is installed in `devDependencies` but **not yet wired into the config**. Wire it in before adding more React Query usage if rule enforcement matters.
- `old-eslint.config.js` is the previous legacy config, kept for reference only. Don't edit it; don't import from it.

## TypeScript

Key `tsconfig.json` settings:

- `"strict": true`
- `"jsx": "react-jsx"` — no need to `import React` in every file
- `"moduleResolution": "bundler"`
- `"target"` and `"module"`: ES6

**Important:** there is no `@typescript-eslint` parser configured. This means:

- TypeScript errors surface at **build time via `ts-loader`**, not through ESLint.
- ESLint rules that depend on type information (e.g. `no-floating-promises`) won't work without adding the parser.

## Styling pipeline

PostCSS config is in `postcss.config.js`. Tailwind 4 + DaisyUI 5 plug in here. The CSS layer structure itself is documented in `ARCHITECTURE.md` under "Styling".

## Environment variables

Injected at build time by `dotenv-webpack`. Create the relevant file before starting:

| File | Used by |
|---|---|
| `.env.development` | `npm start` |
| `.env.production` | `npm run build`, `npm run start:prod` |

Key variables:

```env
REACT_APP_API_URL=http://localhost:3001
REACT_APP_SIGNALR_HUB_URL=http://localhost:5050/hub
```

Because these are baked in at build time, **changing a `.env` value requires restarting the dev server** — hot reload won't pick it up.
