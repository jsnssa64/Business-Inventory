# Business Inventory

A monorepo for a business inventory management platform: a .NET 8 API built with DDD, CQRS, and event sourcing; a React SPA; and a set of supporting services for auth, notifications, and deployment.

## Repository Layout

```
Business-Inventory/
├── Inventory-UI/          # React 19 + TypeScript SPA (Webpack 5, Tailwind 4, DaisyUI 5)
├── Inventory-API/         # ASP.NET Core 8 Web API — DDD + CQRS + event sourcing
├── Inventory-DB/          # SQL Server container with DACPAC schema + admin seeding
├── AuthenticationService/ # gRPC service for seeding the admin user (scaffolded, WIP)
├── Grpc.Shared/           # Shared gRPC contract library (proto is a placeholder, WIP)
├── NotificationService/   # SignalR hub + MassTransit/RabbitMQ consumer (WIP, doesn't compile)
├── Infrastructure/        # Docker Compose (local dev) + Kubernetes/Helm (CD)
├── deploy-scripts/        # Convenience wrappers around Helm deploys and secret sync
├── glossary/              # Ubiquitous language glossary — shared domain vocabulary
```

Each directory has its own `README.md` with full details; this file is a map and quick-start. Several services (`AuthenticationService`, `Grpc.Shared`, `NotificationService`) are early scaffolding — see their READMEs and `TODO.md` files for current gaps before relying on them.

## Architecture

### Inventory API — DDD + CQRS + Event Sourcing

All state mutations flow through **MediatR commands/queries**. The domain uses **aggregate roots** (`InventoryAggregate`, `ProductAggregate`, `OrderAggregate`) that raise **domain events**, persisted to **KurrentDB** (EventStoreDB) for the audit log and published to **RabbitMQ** via MassTransit for downstream processing. Read paths use **Dapper** directly against SQL Server — no ORM. **FusionCache** wraps read-heavy queries with a 2-minute default TTL.

Authentication issues JWT tokens as **HTTP-only cookies** (`authAccessToken` 20 min, `authRefreshToken` 30 days), RSA-256 signed. Roles are hierarchical: `Guest (1) < User (2) < Admin (3)`.

See `Inventory-API/README.md` for the full endpoint list, configuration keys, and tech stack.

### Inventory UI — React Query + React Hook Form + Zod

Server state lives in **TanStack React Query**. Forms use **React Hook Form** with **Zod** schemas. HTTP goes through **Axios**; real-time updates arrive via a **SignalR** connection to `NotificationService`. Routing is React Router v7 (`/Inventory/:userId`, `/Inventory/User`).

See `Inventory-UI/README.md`.

### Inventory DB

A SQL Server 2022 container whose schema is deployed from a DACPAC (`InventoryDb` SQL project) at startup, alongside `AuthenticationService` for admin seeding.

See `Inventory-DB/README.md`.

### AuthenticationService + Grpc.Shared

`AuthenticationService` is a gRPC service meant to seed the initial admin user into `Inventory-DB` at container startup, using the contract defined in `Grpc.Shared`. Both are early scaffolding — the proto is still the default Hello World template and no RPC methods are implemented.

See `AuthenticationService/README.md` and `Grpc.Shared/README.md`.

### NotificationService

Consumes inventory events off RabbitMQ via MassTransit (`SMSNotificationConsumer`, concurrency limit 1000, redelivery at 5/15/30 min) and is meant to push them to browser clients over SignalR at `/hub`. Currently does not compile — see the service README for what's broken.

See `NotificationService/README.md`.

## Common Commands

### UI (`Inventory-UI/`)
```bash
npm install
npm start          # Dev server on localhost:3000 with hot reload
npm run build      # Production build to dist/
npm run lint       # ESLint
npm run test       # Jest (watch mode)
```

### API (`Inventory-API/`)
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

See `Infrastructure/README.md` for the full service/port table and environment variable reference.

## Deployment

Helm charts live in `Inventory-API/Chart/`. Deploy with:

```bash
./Chart/deploy.sh --environment <dev|staging|prod> --release-name <name>
```

Required Kubernetes secrets: `db-credentials`, `rabbitmq-credentials`, `kurrentdb-credentials`, `security-keys`.

`deploy-scripts/` has convenience wrappers (`deploy-component.sh`, `deploy-secret.sh`) for deploying individual Helm components across services and environments. See `deploy-scripts/README.md` and `Infrastructure/README.md` for the full Kubernetes/Helm layout.

## Domain Vocabulary

`glossary/` is the source of truth for domain terms used across services (e.g. `Product` vs `Item`, `User` vs `Customer`), so naming stays consistent across the monorepo. See `glossary/README.md` for the file format and contribution rules.

## Project Status

There's no CI/CD automation yet (no GitHub Actions workflows; `Infrastructure/CI/Terraform` is an empty stub) and several cross-service integrations are unfinished — gRPC auth seeding, the notification pipeline, webhook dispatch, and email confirmation. See `TODO.md` for the full cross-service punch list, and each service's own `TODO.md` for service-scoped gaps.
