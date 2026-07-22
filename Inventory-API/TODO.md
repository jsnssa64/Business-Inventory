# Inventory-API TODO

Mark done with `[x]` or `[done]`.

---

## Authentication & Security

- [ ] Cookie `Secure` flag is hardcoded to `false` in `SecurityService`. Must be `true` before any non-localhost deployment.
- [ ] Cookie expiry is hardcoded to 1 day for both tokens, ignoring config values. Access token should expire at 20 min; refresh token expiry should come from `Security:RefreshToken` config.
- [ ] `SecurityController` has an exposed `CreatePassword` endpoint with no auth guard — dev-only, must be removed or protected.

---

## User Management

- [ ] FluentValidation validators are defined for several DTOs but `AddValidatorsFromAssembly` is never called in `ServiceCollectionExtensions.cs`. No validator runs on any incoming request.
- [ ] Missing validators entirely for: `UpdateUserDetailsDTO`, `UserWithRoleRegisterDTO`, `ResetPasswordDTO`, `UserNewPassword`, `UsersRoleDTO`, `UserEmailDTO`.
- [ ] `DisableUser` and `EnableUser` controller actions call `SetUserStatus` without `await` — tasks are silently discarded.
- [ ] Role validation check is inverted in both `UserController.RegisterUserWithRole` and `UserService.GetUserDetails` — valid roles are rejected instead of invalid ones.
- [ ] Email confirmation flow not implemented. Registration auto-activates users as a temporary workaround (marked TODO in `UserService`).

---

## Products

- [ ] `ProductRepository.GetProductById` returns an empty unmapped object every time. The Dapper result is fetched but the mapping call is commented out. Endpoint is broken.
- [ ] `ProductRepository.GetProducts` maps to a flat type that doesn't match the nested `Product` domain model structure.
- [ ] No validators for any product DTOs (`ProductDTO`, `UpdateProductDTO`, `PriceDTO`, `ProductIdDTO`).

---

## Inventory — Event Streaming

- [ ] `InventoryRepository.AppendEventStream` and `ReadEventStream` both throw `NotImplementedException`. The streaming path for inventory is dead.
- [ ] `InventoryController.AddToStream` and `RemoveFromStream` are marked `[Obsolete]` and `private`. The event streaming design in the controller was abandoned mid-build.

---

## Webhooks

- [ ] `IWebhookService.PostWebhookBySubscription` is implemented but never called anywhere — nothing triggers outbound webhook dispatch after any event.
- [ ] `WebhookController` calls `IWebhookRepository` directly, bypassing the service layer entirely.

---

## CQRS / Event Sourcing

- [ ] MediatR scans only the `InventoryApi` presentation assembly. Handlers in the `Services` assembly (email notification, user created notification) are never discovered — all `_mediator.Publish` calls for these go unhandled.
- [ ] `IEventStoreDbRepository` is an empty interface. `EventStoreDbRepository` is not registered in DI.
- [ ] `Aggregate.Commit()` only flushes when 50 events are queued. Individual commands never persist. The actual KurrentDB write is not implemented (placeholder comment in the method).
- [ ] `ProductAggregate.Apply(ProductCreated)` body is entirely commented out — the aggregate does nothing to its own state when a product is created.

---

## Orders

- [ ] `OrderAggregate` exists with domain events (`CreateOrder`, `OrderStatusChanged`) but there is no controller, service, repository, or DTOs for orders. The aggregate is entirely orphaned.

---

## Roles

- [ ] `RoleRepository` is marked `[Obsolete]` throughout. Role data is now served from in-memory constants (`Roles.AllRoles`). The repository and the DB role tables are dead code — decide whether to remove or restore.
- [ ] `CreateRole` endpoint is `[Obsolete]` and `private` — role creation has no working replacement.
