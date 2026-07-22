# Code Practices — Inventory-API

> Conventions and recipes for working in this codebase. The hard rules in `CLAUDE.md` take precedence; anything here can be deviated from with good reason.

## Conventions
These are suggestions and conventions to consider on any new users or mistakes from existing users to get caught early before PRing into main and being missed or having to be repeated

### Architecture

- **Controllers are thin.** Delegate to a service via constructor injection. No business logic, no data access, no caching.
- **Caching lives at the service layer.** Use `FusionCache`. Don't cache in controllers or repositories.
- **All DB access is through stored procedures**, called via `DapperDbConnectionFactory`. No inline SQL, no ORM-generated queries.
- **Domain events are raised by aggregate roots**, not by services. Services orchestrate; aggregates decide what happened.
- **Connection string keys are referenced from `Shared/Constants/DatabaseConnections.cs`** — this is the single source of truth.
- **Read models (`Services/DataModel/`) are distinct from domain entities (`Domain/Entities/`).** Don't mix them. Return read models from controllers; mutate domain entities inside aggregates.
- **Validation happens via FluentValidation** in `InventoryApi/Validation/`, not inside controllers or services.
- **Authorization is declarative** via `[MinimumRole(Roles.X)]`. Don't check roles imperatively inside actions unless the rule genuinely can't be expressed as a minimum role.

### Coding Conventions

-   **Conditional Refactoring.** Else statements within an if/else conditional should be minimised as much as possible as patterns like if inversions can minimise the need of an else. Other things to be wary of is when an if statement has multiple nested if statement this means something may need refactoring futher and could be a sign of a function doing more then it should
-   **Functions being minimal.** Functions should have one purpose, if you are grouping multiple purposes to one function this could be a flag of a bigger issue
-   **Duplication Logic.** Utilities should be used when logic is repeated unless the logics intentions are different but do the same thing but even then this may be a sign that you need to clarify why is that the case
-   **NUll vs Default Values To Define an empty value** (Note, not sure if this is an issue or not that needs considering or this is just a situational issue that doesnt need looking at


## Recipes

### Adding a new endpoint

1. Create a controller in `InventoryApi/Controllers/`, inheriting `BaseController`.
2. Add request/response DTOs under `InventoryApi/DTOs/<Domain>/`.
3. Add a FluentValidation rule under `InventoryApi/Validation/`.
4. Decorate the action (or controller) with `[MinimumRole(Roles.User)]` or `[MinimumRole(Roles.Admin)]`.
5. Inject the relevant service — never call repositories directly from controllers.

### Adding a new service or repository

1. Define the interface and implementation under `Services/Service/<Domain>/` or `Services/Repository/<Domain>/`.
2. Register it in `AddApiServices()` — or add a new extension method if the concern is genuinely standalone (webhook-style).
3. Repositories call stored procedures only, via `DapperDbConnectionFactory`. No inline SQL.

### Adding a new domain event

1. Define the event class under `Domain/Events/<Domain>/`. Events are immutable.
2. Raise it from the aggregate root (not from a service).
3. Persist it via `IEventStoreDbRepository`.
4. Publish downstream via `EventNotificationService` (MassTransit → RabbitMQ) if other services need to react.

## Anti-patterns

<!-- Fill these in as you encounter them. Real examples beat abstract advice. -->

- <!-- e.g. Reaching into a repository from a controller "just for a quick read" — always go through the service so caching and authorization stay consistent. -->
- <!-- e.g. Raising a domain event from a service because "the aggregate doesn't have the data" — refactor the aggregate instead. -->
- <!-- e.g. Adding a new connection string as a magic string instead of extending `DatabaseConnections.cs`. -->

## When in doubt

- Match the style of the surrounding code in the same layer.
- Check `ARCHITECTURE.md` if the question is about *where* something belongs.
- If still unsure, ask.
