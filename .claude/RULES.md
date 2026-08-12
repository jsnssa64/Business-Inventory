# Architecture rules for this project

## Architecture style

This is a multi-service monorepo; architecture style varies by subproject.

### Inventory-API
Clean Architecture + DDD. Layers: Presentation (`InventoryApi/`) → Application (`Services/`) → Domain (`Domain/`) → Shared (`Shared/`). CQRS via MediatR, aggregate roots raise domain events, events persisted to KurrentDB and published via MassTransit/RabbitMQ.

### Inventory-UI
Component-based React 19 + TypeScript SPA. Server state via React Query, forms via React Hook Form + Zod.

### NotificationService
SignalR hub + MassTransit consumer. (Architecture style not yet discussed.)

### AuthenticationService
(Architecture style not yet discussed.)

### Inventory-DB
SQL Server setup + seeding service. (Architecture style not yet discussed.)

## Confirmed rules

<!-- appended here as they're established, one per entry -->
<!-- format: - RULE. Confirmed YYYY-MM-DD. -->

## Open questions

- What architecture style (if any) applies to NotificationService, AuthenticationService, and Inventory-DB?

## Unconfirmed observations (not rules - candidates only)

- Inventory-API `Domain/` is mid-reorganization into `Aggregates/`, `Entities/`, `Events/`, `ValueObjects/` — many files show as modified/deleted in git status, consistent with an in-progress move.
- Inventory-API `Domain/ValueObjects/User/` holds 8 files (`EmailConfirmationModel`, `UserAddress`, `UserClaims`, `UserDetails`, `UserId`, `UserIdentity`, `UserLogin`, `UserRole`, `UserWithPassword`) — not all of these look like true value objects; some may be entities, auth/claims models, or DTOs that don't belong in Domain.
- Inventory-API `Services/Service/SecurityService/Models/` (`KeyType`, `Security`, `SecurityLevel`, `Token`) looks like domain modeling stranded in the Application layer instead of Domain.
- Inventory-UI groups feature components under `components/` (`Inventory`, `Navigation`, `Paging`, `ProfileNavBar`, `login`, `profile`) and keeps generic UI primitives separate under `rootComponents/` (`Icon`, `dataTable`, `dropDown`, `modal`, `tile`).
- Inventory-UI `models/` is split into `data/` and `ui/` subfolders.
