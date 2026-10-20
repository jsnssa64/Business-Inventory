# Architecture rules for this project

## Architecture style
Clean Architecture + DDD. Work in progress - the codebase is mid-migration towards this,
so existing code is not yet conformant.

Target bounded contexts (see `glossary/CONTEXT-MAP.md`): Supplier Registry, Supplier Catalogue,
Supplier Pricing, Ordering, Stock, Notifications, Identity. Inventory-API is intended to hold Stock +
Ordering; Supplier Registry, Hosted Catalogue and Supplier Pricing are separate services of ours, and
suppliers' own catalogues are external. Today's code does not yet follow these boundaries.

## Confirmed rules
<!-- appended here as they're established, one per entry -->
<!-- format: - RULE. Confirmed YYYY-MM-DD. -->

## Open questions
<!-- placement questions that came up but weren't resolved into a rule -->

## Unconfirmed observations (not rules - candidates only)
<!-- precedent noticed in the code, not yet confirmed as intentional -->
- Inventory-API is split into `Domain` (aggregates, entities, events), `Services` (services + repositories + data models), `InventoryApi` (controllers, DTOs, validation, auth, middleware).
- `Services` currently mixes application logic (`Service/`) with infrastructure (`Repository/`, `DataModel/`, `EventStoreDbRepository/`) - candidate split into Application and Infrastructure.
- `Inventory-API/Infrastructure` and `Inventory-API/Shared` folders exist but are empty - likely the intended Infrastructure/shared-kernel homes.
- `Aggregate.commit()` decides when to flush to the event store (batches at 50 uncommitted events) - persistence timing lives in the domain rather than a repository.
- Value objects and identities (`ProductIdentity`, `Price`, `ProductInfo`) live under `Domain/Entities/<Aggregate>/`.
- Domain events are grouped per aggregate under `Domain/Events/<Aggregate>/`, each with a base `<Aggregate>Event`.
- Domain classes use the stale namespace `InventoryApi.Model.Action` / `InventoryApi.Model.Events` rather than `Domain.*`.
- `InventoryApi/Factory/ConnectionFactory.cs` and `Extensions/ServiceCollectionExtensions.cs` reference Dapper/KurrentDB/EF directly from the API project.
- Domain project depends on MediatR.
