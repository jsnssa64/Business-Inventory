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
- CQRS is decided per area, not per endpoint: an event-sourced area uses CQRS via MediatR (commands/queries + handlers) for all of its reads and writes; every other area uses application services directly. Confirmed 2026-11-02.
- The HTTP request DTO lives in `InventoryApi/` and never leaves the controller. The controller copies body, route and claims values into an input type owned by `Services/` (service input record or MediatR command, primitives or `Services/`-owned lightweight records) and passes that inward. Confirmed 2026-11-02.
- Only the service/handler builds value objects and aggregates; controllers never construct or receive domain types. Services/handlers return output DTOs owned by `Services/`, never entities or aggregates. Controllers map service output DTOs to response types owned by `InventoryApi/`; service DTOs are never returned as HTTP responses directly. Confirmed 2026-11-02.
- FluentValidation lives in `Services/` and validates the service input / command, not the HTTP DTO, so every entry point (HTTP, consumers, jobs) is validated. Service path: the service calls its validator explicitly. CQRS path: a MediatR pipeline behaviour runs it. Confirmed 2026-11-02.
- No validation overlap: FluentValidation checks shape only (required, lengths, formats); domain value objects and aggregates check business rules only, returned as `Result` failures. Confirmed 2026-11-02.
- Service inputs: more than 3 values → an input record; otherwise primitive parameters. Exception: values whose meaning would be lost as a primitive (e.g. identities such as a product ID) are passed as lightweight records owned by `Services/`, even when alone. These are not domain types; the service converts them to domain identities. Named per the `Services/` naming convention. Confirmed 2026-11-02.
- Naming: `InventoryApi/` uses `<Verb><Noun>Request` and `<Noun>Response`. `Services/` uses `<Verb><Noun>Input` for service inputs, `<Verb><Noun>Command` / `<Verb><Noun>Query` and `<Command/Query>Handler` for CQRS, `<InputType>Validator` for validators, `<Noun>Dto` for outputs, and `<Noun>IdInput` for typed IDs (e.g. `ProductIdInput`). Typed IDs are inputs only; output DTOs contain primitives only. Confirmed 2026-11-02.

## Open questions
<!-- placement questions that came up but weren't resolved into a rule -->
- Shape of the validation error in `Result`: must carry multiple field errors (field + message), unlike single domain errors. Not designed yet.
- Boundary test for "shape vs business rule" (e.g. is a max name length shape or business?). No agreed test yet.

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
