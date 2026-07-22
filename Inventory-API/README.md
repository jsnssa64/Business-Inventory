# Inventory API

A .NET 8.0 REST API for business inventory management, built with Domain-Driven Design, CQRS, and Event Sourcing patterns.

## Overview

Inventory API handles product catalogue management, stock level tracking, user management with role-based access control, and webhook-based event notifications. Domain events are persisted in an event store (KurrentDB) for full audit trails, with RabbitMQ enabling asynchronous event distribution.

## Architecture

```
InventoryApi/          Presentation layer — controllers, middleware, auth, DTOs
Services/              Business logic — domain services, repositories (Dapper)
Domain/                Domain layer — aggregates, entities, domain events
Shared/                Cross-cutting constants and utilities
Chart/                 Helm charts for Kubernetes deployment
```

**Key patterns:**
- **CQRS** via MediatR — commands and queries separated in-process
- **Event Sourcing** — all state changes stored as immutable events in KurrentDB
- **Domain-Driven Design** — aggregates (`InventoryAggregate`, `ProductAggregate`, `OrderAggregate`) own their domain logic
- **Repository pattern** — Dapper-backed repositories per aggregate root
- **JWT + HTTP-only cookies** — RSA-256 signed tokens stored as `authAccessToken` (20 min) and `authRefreshToken` (30 days)
- **Hierarchical RBAC** — `Guest (1) → User (2) → Admin (3)` enforced via `[MinimumRole]` attribute

## Technology Stack

| Concern | Library |
|---|---|
| Framework | ASP.NET Core 8.0 |
| Data access | Dapper 2.1, Microsoft.Data.SqlClient 6.0 |
| Event store | KurrentDB.Client 1.3 (gRPC) |
| Message broker | MassTransit 8.4 + RabbitMQ |
| Mediator / CQRS | MediatR 12.5 |
| Caching | FusionCache 2.6 (2-minute default TTL) |
| Validation | FluentValidation 11 |
| Auth | JwtBearer 8.0, BCrypt.Net |
| Docs | Swashbuckle / Swagger |
| Container | Docker (multi-stage), Helm / Kubernetes |

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server on `localhost:1400`
- KurrentDB on `localhost:2113`
- RabbitMQ on `localhost`

All three can be started with the Docker Compose files in `../Infrastructure/CI/Docker/`.

## Getting Started

```bash
# Restore dependencies
dotnet restore

# Run the API (from the repo root or the InventoryApi/ directory)
dotnet run --project InventoryApi/InventoryApi.csproj
```

The API starts on `http://localhost:5000` / `https://localhost:5001`.  
Swagger UI: `http://localhost:5000/swagger`

### Docker

```bash
# Build image
docker build -t inventory-api:latest .

# Run container (exposes port 8080)
docker run -p 8080:8080 inventory-api:latest
```

### Kubernetes (Helm)

```bash
./Chart/deploy.sh --environment dev --release-name inventory-api
```

Environment-specific value files (`Chart/env/dev.values.yaml`, `staging.values.yaml`, `prod.values.yaml`) are selected by the `--environment` flag. Required secrets: `db-credentials`, `rabbitmq-credentials`, `kurrentdb-credentials`, `security-keys`.

## Configuration

Key settings in `appsettings.json` / `appsettings.Development.json`:

| Key | Description |
|---|---|
| `ConnectionStrings:InventoryDb` | SQL Server connection string |
| `ConnectionStrings:KurrentDb` | KurrentDB gRPC connection string (`kurrentdb://...`) |
| `ConnectionStrings:RabbitMQ` | RabbitMQ connection string |
| `Security:AccessToken:Key` | Base64-encoded RSA key for access token signing (20 min) |
| `Security:RefreshToken:Key` | Base64-encoded RSA key for refresh token signing (30 days) |
| `Security:ConfirmationToken:Key` | Base64-encoded RSA key for email confirmation tokens |
| `Security:ResetPasswordToken:Key` | Base64-encoded RSA key for password reset tokens |
| `Security:Issuer` / `Security:Audience` | JWT issuer and audience claims |

## API Endpoints

### Authentication
Public endpoints — no token required.

| Method | Path | Description |
|---|---|---|
| `POST` | `/User/Register` | Register a new user |
| `POST` | `/User/Login` | Login (sets auth cookies) |
| `GET` | `/User/Logout` | Clear auth cookies |
| `POST` | `/User/ForgottenPasswordByEmail` | Trigger password reset by email |
| `GET` | `/User/ForgottenPasswordByUsername` | Trigger password reset by username |
| `POST` | `/User/ResetPassword` | Complete password reset |
| `GET` | `/User/Confirmation` | Confirm email address |

### Products
Requires `User` role or higher.

| Method | Path | Description |
|---|---|---|
| `GET` | `/Product/GetProducts` | List all products for the authenticated user |
| `GET` | `/Product/GetProductById` | Get product by ID |
| `POST` | `/Product/AddProduct` | Create a product |
| `POST` | `/Product/UpdateProduct` | Update a product |
| `GET` | `/Product/RemoveProduct` | Delete a product |

### Inventory
Requires `User` role or higher.

| Method | Path | Description |
|---|---|---|
| `GET` | `/Inventory/GetInventory` | List all inventory items |
| `GET` | `/Inventory/GetInventoryItemByProductId` | Get inventory item by product |
| `POST` | `/Inventory/UpdateItemInInventory` | Update stock level |

### Users
Mixed — some endpoints require `Admin` role.

| Method | Path | Access | Description |
|---|---|---|---|
| `GET` | `/User/GetUser` | User | Current user profile |
| `GET` | `/User/GetUserDetails` | User | Full current user details |
| `POST` | `/User/Update` | User | Update own details |
| `POST` | `/User/ChangePassword` | User | Change own password |
| `GET` | `/User/GetUsers` | Admin | List all users |
| `GET` | `/User/GetUserDetailsByUser` | Admin | Get any user's details |
| `POST` | `/User/RegisterUserWithRole` | Admin | Register user with a specific role |
| `POST` | `/User/AssignUserRole` | Admin | Change a user's role |
| `GET` | `/User/Disable` | Admin | Disable a user account |
| `GET` | `/User/Enable` | Admin | Enable a user account |

### Roles

| Method | Path | Description |
|---|---|---|
| `GET` | `/Role/GetRoles` | List all roles |
| `GET` | `/Role/GetDefaultRole` | Get the default role |

### Webhooks

| Method | Path | Description |
|---|---|---|
| `GET` | `/Webhook/RegisterWebhook` | Subscribe a URL to an inventory event type |

## Authentication Flow

1. `POST /User/Login` — validates credentials, issues RSA-256 signed JWT tokens stored in `authAccessToken` (20 min) and `authRefreshToken` (30 days) HTTP-only cookies.
2. Subsequent requests pass through `JwtCookieAuthenticationMiddleware`, which reads and validates the access token cookie automatically.
3. On expiry the refresh token is used to issue a new access token.
4. Passwords are hashed with BCrypt before storage.
