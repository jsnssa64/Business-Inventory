# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Repository Layout

```
Business-Inventory/
├── Inventory-UI/          # React 19 + TypeScript SPA (Webpack 5, Tailwind 4, DaisyUI 5)
├── Inventory-API/         # ASP.NET Core 8 Web API
│   ├── InventoryApi/      # Presentation: controllers, middleware, DTOs, validators
│   ├── Services/          # Application layer: CQRS handlers, Dapper repositories
│   ├── Domain/            # Domain layer: aggregates, entities, domain events
│   └── Shared/            # Cross-cutting: constants, utilities
├── Inventory-DB/          # SQL Server setup with seeding service
├── NotificationService/   # SignalR hub + MassTransit consumer for real-time alerts
└── Infrastructure/
    ├── CI/Docker/         # Docker Compose files (one per service, merged at startup)
    └── CD/                # Kubernetes manifests + Helm charts
```

## Common Commands

### UI (Inventory-UI/)
```bash
npm install
npm start          # Dev server on localhost:3000 with hot reload
npm run build      # Production build to dist/
npm run lint       # ESLint
npm run test       # Jest (watch mode)
```

### API (Inventory-API/)
```bash
dotnet restore
dotnet run --project InventoryApi/InventoryApi.csproj   # http://localhost:5000, Swagger at /swagger
dotnet build -c Release
dotnet test                                               # from solution root
```

### Full Stack via Docker Compose
```bash
cd Infrastructure/CI/Docker

# One-time: create shared networks
docker network create app-shared-network
docker network create internal-shared-network

# Start everything (compose files are split by service)
docker-compose \
  -f compose.base.yml \
  -f compose.db.yml \
  -f compose.eventstoredb.yml \
  -f compose.rabbitmq.yml \
  -f compose.api.yml \
  -f compose.ui.yml \
  up -d
```

Default `.env` values for local dev: `MSSQL_SA_PASSWORD=test123!`, `DEFAULT_ADMIN_PASSWORD=test123!`, `RABBITMQ_DEFAULT_PASS=test123!`.

## Architecture

### API — DDD + CQRS + Event Sourcing

All state mutations flow through **MediatR commands/queries** (CQRS). The domain uses **aggregate roots** (`InventoryAggregate`, `ProductAggregate`, `OrderAggregate`) that raise **domain events**. Every event is persisted to **KurrentDB** (EventStoreDB) for the audit log and then published to **RabbitMQ** via MassTransit for async downstream processing (e.g., NotificationService).

Read paths use **Dapper** directly against SQL Server — no ORM. **FusionCache** wraps read-heavy queries with a 2-minute default TTL.

Service registration is organized in `ServiceCollectionExtensions.cs`:
- `AddSecurityServices()` — JWT utility + BCrypt security service
- `AddApiServices()` — domain services + Dapper repositories
- `AddEventServices()` — MediatR + MassTransit/RabbitMQ + KurrentDB client
- `AddMemoryServices()` — FusionCache
- `AddDatabaseServices()` — Dapper connection factory

### Authentication

JWT tokens are issued as **HTTP-only cookies** (`authAccessToken` 20 min, `authRefreshToken` 30 days), signed with RSA-256. `JwtCookieAuthenticationMiddleware` validates the access token on every request. Roles are hierarchical integers: `Guest (1) < User (2) < Admin (3)`, enforced via `MinimumRoleAttribute`.

### UI — React Query + React Hook Form + Zod

Server state lives in **TanStack React Query** (v5). Forms use **React Hook Form** with **Zod** schemas for validation. HTTP calls go through an **Axios** instance. Real-time updates come from a **SignalR** connection to NotificationService. Routing is React Router v7 with two primary route groups: `/Inventory/:userId` and `/Inventory/User`.

### NotificationService

Listens on RabbitMQ via MassTransit (`SMSNotificationConsumer`, concurrency limit 1000, redelivery at 5/15/30 min). Pushes to connected browser clients over SignalR at `/hub`.

## Deployment

Helm charts live in `Inventory-API/Chart/`. Use `./Chart/deploy.sh --environment <dev|staging|prod> --release-name <name>`. Required Kubernetes secrets: `db-credentials`, `rabbitmq-credentials`, `kurrentdb-credentials`, `security-keys`.
