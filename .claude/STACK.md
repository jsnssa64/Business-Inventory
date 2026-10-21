# Stack

Multi-service monorepo; stack varies by subproject.

## Inventory-API

| Role | Library |
|---|---|
| API layer | ASP.NET Core 8 Web API (`Microsoft.NET.Sdk.Web`) |
| Command/query dispatch (CQRS) | MediatR 12.5.0 |
| Persistence (reads/writes) | Dapper 2.1.72 + Microsoft.Data.SqlClient 6.0.1, via stored procedures |
| Event sourcing | KurrentDB.Client 1.3.1 (`EventStore.Client.Grpc.Streams` 23.3.8 also referenced in `InventoryApi.csproj` — unconfirmed whether both are still needed) |
| Async messaging | MassTransit.RabbitMQ 8.4.1 |
| Caching | ZiggyCreatures.FusionCache 2.6.0 |
| Auth (tokens) | Microsoft.AspNetCore.Authentication.JwtBearer 8.0.15, System.IdentityModel.Tokens.Jwt 7.5.0 |
| Password hashing | BCrypt.Net-Next 4.1.0 |
| Validation | FluentValidation 11.11.0 |
| API docs | Swashbuckle.AspNetCore 6.6.2 |

## Inventory-UI

| Role | Library |
|---|---|
| Framework | React 19 + TypeScript, bundled with Webpack 5 |
| Server state | TanStack React Query 5 |
| Forms + validation | React Hook Form 7 + Zod 3 |
| Real-time | @microsoft/signalr 8.0.7 |
| Styling | Tailwind CSS 4 + DaisyUI 5 |
| Routing | react-router-dom 7 |
| HTTP client | Axios |

## NotificationService

| Role | Library |
|---|---|
| Messaging consumer | MassTransit (RabbitMQ) |
| Real-time push | SignalR (server-side hub) |

## AuthenticationService / Inventory-DB

Not yet inventoried — revisit when those subprojects are in scope.
