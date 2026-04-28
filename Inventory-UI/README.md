# Inventory UI

React 19 + TypeScript single-page application for business inventory management. Bundled with Webpack 5, styled with Tailwind CSS 4 and DaisyUI 5.

## Tech Stack

| Concern | Library |
|---|---|
| UI framework | React 19 + TypeScript 5.8 |
| Bundler | Webpack 5 |
| Styling | Tailwind CSS 4 + DaisyUI 5 |
| Routing | React Router DOM 7 |
| Server state | TanStack React Query 5 + Devtools |
| Forms | React Hook Form 7 + Zod 3 |
| HTTP client | Axios 1.8 |
| Real-time | Microsoft SignalR 8 |

## Getting Started

```bash
npm install
npm start          # Dev server on http://localhost:3000 (hot reload)
npm run build      # Production bundle to dist/
npm run lint       # ESLint
npm run test       # Jest (watch mode)
```

For a production preview:

```bash
npm run start:prod
```

## Environment Variables

Webpack injects environment variables at build time via `dotenv-webpack`. Create the relevant file before running:

- `.env.development` — loaded by `npm start`
- `.env.production` — loaded by `npm run build` / `npm run start:prod`

Example variables (add your own per environment):

```env
REACT_APP_API_URL=http://localhost:3001
REACT_APP_SIGNALR_HUB_URL=http://localhost:5050/hub
```

## Routes

| Path | Description |
|---|---|
| `/Inventory/:userId` | Inventory view for a specific user |
| `/Inventory/User` | Current user's inventory/profile |

The dev server uses `historyApiFallback: true`, so all routes are served from `index.html`.

## Docker

A multi-stage Dockerfile is included. The production stage uses `serve` to host the static bundle:

```bash
docker build -t inventory-ui .
docker run -p 3000:3000 inventory-ui
```

Or start via Docker Compose — see `../Infrastructure/README.md`.
