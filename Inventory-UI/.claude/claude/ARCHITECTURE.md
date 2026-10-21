# Architecture — Inventory-UI

How the source is organised and how data flows through the app.

## Data flow

```
Component  →  hook (src/hooks/)  →  service (src/api/services/)  →  Axios / SignalR  →  API
```

Each step has a single responsibility:

- **Components** render and handle user interaction. They never call services directly.
- **Hooks** wrap React Query (`useQuery`/`useMutation`) and own cache keys, retries, and staleness.
- **Services** are plain async functions that wrap Axios calls. They know URLs and request shapes, nothing about React.
- **Axios instance** (`src/api/axios/`) centralises base URL, interceptors, and auth/cookie config.

SignalR follows the same shape: components consume the SignalR wrapper components in `src/api/signalR/`, which expose typed events upward.

## Source layout

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

## Components vs rootComponents

- **`components/`** — feature-specific. Tied to a domain (Inventory, Profile, Login). Not reused outside their feature.
- **`rootComponents/`** — generic, reusable building blocks (DataTable, Modal, Tile, Icon, DropDown). No knowledge of business domain.

When building something new, ask: would another feature use this? If yes, it belongs in `rootComponents/`. If it's specific to inventory or products, it belongs in `components/`.

## Models: data vs ui

- **`models/data/`** — shapes returned by the API. Mirror the server contracts.
- **`models/ui/`** — types that only exist client-side (form models, permission flags, derived view types).

Don't mix them. If you need to transform an API response into something UI-friendly, define both types and convert at the hook layer.

## Styling

Tailwind CSS 4 + DaisyUI 5 processed through PostCSS. Custom CSS is layered:

| Layer | File | Purpose |
|---|---|---|
| Base | `style/base.css` | Resets and base element styles |
| Components | `style/components.css` | Component-level classes (imports from `style/components/`) |
| Utilities | `style/utility.css` | Utility overrides (imports from `style/utilities/`) |
| Entry | `style/index.css` | Imports all three layers + Tailwind directives |

When adding styles:

- Standard UI elements → use DaisyUI component classes (`btn`, `modal`, `table`, etc.)
- One-off layout/spacing → Tailwind utilities inline
- Repeated patterns that DaisyUI doesn't cover → add a class under `style/components/`
- Overriding Tailwind/DaisyUI behaviour → add to `style/utilities/`
