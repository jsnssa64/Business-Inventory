# Architecture — Inventory-API

How the system is organised and how the pieces fit together.

## Layer overview

Three layers, dependencies flow inward:

- **Presentation** (`InventoryApi/`) — HTTP, DTOs, validation, auth middleware
- **Application** (`Services/`) — domain services, repositories, read models
- **Domain** (`Domain/`) — aggregates, entities, value objects, domain events

`Shared/` is cross-cutting (constants, user utilities) and may be referenced from any layer.

## Folder layout

```
Inventory-API/
├── InventoryApi/          # Presentation layer
│   ├── Controllers/       # HTTP endpoints — all inherit BaseController
│   ├── Authorization/     # [MinimumRole] attribute
│   ├── DTOs/              # Request/response models per domain
│   ├── Extensions/        # ServiceCollectionExtensions.cs, MiddlewareExtension.cs
│   ├── Factory/           # DapperDbConnectionFactory
│   ├── Middleware/        # JwtCookieAuthenticationMiddleware
│   ├── Validation/        # FluentValidation rules
│   └── Program.cs
├── Services/              # Application layer
│   ├── Service/           # Domain services (Inventory, Product, User, Role, Webhook, Security)
│   ├── Repository/        # Dapper repositories + EventStoreDbRepository (KurrentDB)
│   └── DataModel/         # Query-side read models
├── Domain/                # Domain layer
│   ├── Aggregates/        # InventoryAggregate, ProductAggregate, OrderAggregate
│   ├── Entities/          # Inventory, Order, Product, User entities + value objects
│   └── Events/            # Immutable domain events
├── Shared/                # Cross-cutting
│   ├── Constants/         # DatabaseConnections, JWTCookie, Roles
│   └── Utilities/User/    # IUserUtility, UserUtility, UserClaim
└── Chart/                 # Helm charts (dev/staging/prod)
```

## Service registration map

Everything is wired up in `InventoryApi/Extensions/ServiceCollectionExtensions.cs`. Each extension method groups a related set of registrations:

| Extension | What it registers |
|---|---|
| `AddSecurityServices` | `IJWTUtility`, `ISecurityService`, `Security` config section |
| `AddApiServices` | All domain services + Dapper repositories (User, Inventory, Role, Product) |
| `AddWebhook` | `IWebhookService`, `IWebhookRepository` |
| `AddEventServices` | MediatR, MassTransit/RabbitMQ, KurrentDB client |
| `AddMemoryServices` | FusionCache (2-min default TTL, Low priority) |
| `AddDatabaseServices` | `DapperDbConnectionFactory` singleton |
| `AddLoginAuthentication` | JWT cookie auth scheme (`CookieJwtScheme`) |

When adding new services: extend the appropriate existing method, or add a new extension if the concern is genuinely standalone.

## Authentication flow

- `JwtCookieAuthenticationMiddleware` reads `authAccessToken` from an HTTP-only cookie, validates the RSA-256 JWT, and sets `context.User`.
- On access-token expiry it falls back to `authRefreshToken` to issue a new access token transparently.
- `BaseController.Username` extracts the current user from `ClaimTypes.Name`.
- Role hierarchy: `Guest (1) < User (2) < Admin (3)`. Enforced via `[MinimumRole(Roles.X)]` on controllers or actions.
- Cookie names and the auth scheme name are constants in `Shared/Constants/JWTCookie.cs` — reference them, don't hardcode strings.

## Domain events

- Events are immutable and live in `Domain/Events/<Domain>/`.
- They are raised by aggregate roots (never by services directly).
- Persisted to KurrentDB via `IEventStoreDbRepository`.
- Published downstream via `EventNotificationService` (MassTransit → RabbitMQ) for other services to consume.

## Read models vs domain entities

- **Domain entities** (`Domain/Entities/`) carry behaviour and invariants. They're what aggregates mutate.
- **Read models** (`Services/DataModel/`) are flat shapes optimised for queries — they're populated from the query side and returned to controllers.

These are kept deliberately separate. Don't return domain entities from controllers, and don't put behaviour on read models.

## Configuration

Security keys are Base64-encoded RSA keys, configured under the `Security` section:

- `Security:AccessToken:Key`
- `Security:RefreshToken:Key`
- `Security:ConfirmationToken:Key`
- `Security:ResetPasswordToken:Key`

Where they come from:

- **Local dev:** `InventoryApi/appsettings.Development.json`
- **Kubernetes:** injected from the `security-keys` secret (see `Chart/`)

KurrentDB client uses insecure gRPC (`tls=false`) with a 30-second deadline. Dev credentials are `admin/changeit`. The connection string key is `DatabaseConnections.KurrentDb` — see `Shared/Constants/DatabaseConnections.cs` for the canonical list of connection keys.
