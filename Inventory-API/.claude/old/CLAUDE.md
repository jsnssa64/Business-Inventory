# CLAUDE.md — Inventory-API

Guidance for working within the `Inventory-API/` project. The parent `Business-Inventory/CLAUDE.md` covers the full repo layout and full-stack Docker setup.

## Project Structure

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

## Commands

```bash
dotnet restore
dotnet build
dotnet run --project InventoryApi/InventoryApi.csproj   # http://localhost:5000, /swagger
dotnet test
```

## Service Registration Map

Everything is registered in `InventoryApi/Extensions/ServiceCollectionExtensions.cs`:

| Extension | What it registers |
|---|---|
| `AddSecurityServices` | `IJWTUtility`, `ISecurityService`, `Security` config section |
| `AddApiServices` | All domain services + Dapper repositories (User, Inventory, Role, Product) |
| `AddWebhook` | `IWebhookService`, `IWebhookRepository` |
| `AddEventServices` | MediatR, MassTransit/RabbitMQ, KurrentDB client |
| `AddMemoryServices` | FusionCache (2-min default TTL, Low priority) |
| `AddDatabaseServices` | `DapperDbConnectionFactory` singleton |
| `AddLoginAuthentication` | JWT cookie auth scheme (`CookieJwtScheme`) |

Add new services to the appropriate existing extension, or add a new one if it's a standalone concern.

## Adding a New Feature

### New endpoint
1. Create a controller in `InventoryApi/Controllers/`, inherit `BaseController`
2. Add request/response DTOs under `InventoryApi/DTOs/<Domain>/`
3. Add a FluentValidation rule under `InventoryApi/Validation/`
4. Decorate with `[MinimumRole(Roles.User)]` or `[MinimumRole(Roles.Admin)]`

### New service or repository
1. Define interface + implementation in `Services/Service/<Domain>/` or `Services/Repository/<Domain>/`
2. Register in `AddApiServices()` (or a new extension method if it's a distinct concern)
3. Repositories call stored procedures only via `DapperDbConnectionFactory` — no inline SQL

### New domain event
1. Define the event class under `Domain/Events/<Domain>/`
2. Raise it from the aggregate root
3. Persist via `IEventStoreDbRepository`
4. Publish downstream via `EventNotificationService` (MassTransit → RabbitMQ)

## Authentication

- `JwtCookieAuthenticationMiddleware` reads `authAccessToken` from HTTP-only cookies, validates the RSA-256 JWT, and sets `context.User`
- On expiry it falls back to `authRefreshToken` to issue a new access token
- `BaseController.Username` extracts the current user from `ClaimTypes.Name`
- Role hierarchy: `Guest (1) < User (2) < Admin (3)` — enforced via `[MinimumRole]`
- Cookie names and the auth scheme name are constants in `Shared/Constants/JWTCookie.cs`

## Configuration

Security keys (`Security:AccessToken:Key`, `Security:RefreshToken:Key`, `Security:ConfirmationToken:Key`, `Security:ResetPasswordToken:Key`) are Base64-encoded RSA keys.

- **Local dev**: keys live in `InventoryApi/appsettings.Development.json`
- **Kubernetes**: injected from the `security-keys` secret

KurrentDB client uses insecure gRPC (`tls=false`) with a 30-second deadline. Dev credentials are `admin/changeit`. The connection string key is `DatabaseConnections.KurrentDb` (see `Shared/Constants/DatabaseConnections.cs`).

## Conventions

- Controllers are thin — delegate everything to a service via constructor injection
- Caching is applied at the **service layer** via `FusionCache`, not in controllers or repositories
- All DB access uses **stored procedures** via Dapper; no ORM-generated or inline SQL
- Domain events are raised by **aggregate roots**, not services
- `Shared/Constants/DatabaseConnections.cs` is the canonical source of connection string key names
- Read models live in `Services/DataModel/`; they are distinct from the domain entities in `Domain/Entities/`
