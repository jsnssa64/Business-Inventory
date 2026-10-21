# CLAUDE.md — Inventory-API

Guidance for working within the `Inventory-API/` project. The parent `Business-Inventory/CLAUDE.md` covers the full repo layout and full-stack Docker setup.

## Project at a glance

ASP.NET Core API for inventory management. Clean architecture split: **Presentation** (`InventoryApi/`) → **Application** (`Services/`) → **Domain** (`Domain/`). Uses Dapper + stored procedures for persistence, KurrentDB for event sourcing, MassTransit/RabbitMQ for downstream events, FusionCache for caching, and JWT-cookie auth.

## Where to look

- **`ARCHITECTURE.md`** — folder layout, layer responsibilities, service registration map, auth flow, configuration
- **`CODE_PRACTICES.md`** — conventions, adding-a-feature recipes, anti-patterns
- **This file** — commands and the hard rules below

## Commands

```bash
dotnet restore
dotnet build
dotnet run --project InventoryApi/InventoryApi.csproj   # http://localhost:5000, /swagger
dotnet test
```

## Hard rules

These are non-negotiable for this project. If a request would violate one, flag it before proceeding.

- **No inline SQL.** All database access goes through stored procedures via `DapperDbConnectionFactory`.
- **No ORM.** Dapper only.
- **Domain events are raised by aggregate roots, never by services.**
- **Controllers stay thin** — no business logic, delegate to a service.
- **Caching belongs at the service layer**, never in controllers or repositories.
- **Connection string keys live in `Shared/Constants/DatabaseConnections.cs`** — don't hardcode them elsewhere.
