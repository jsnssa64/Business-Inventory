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
- [x] Role validation check is inverted in both `UserController.RegisterUserWithRole` and `UserService.GetUserDetails` — valid roles are rejected instead of invalid ones. Fixed 2026-08-12: `UserController.cs` check negated; `UserService.cs` check removed entirely (redundant once `UserRole` wraps `RoleLevel` — invalid roles are no longer constructible).
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
- [ ] `RoleValidator.cs` (`InventoryApi/Validation/`) is dead code — not registered anywhere (see FluentValidation item above) — and as of 2026-08-12 it also fails to compile: it validates `UserRole.Rolename`, a property that no longer exists now that `UserRole` wraps `RoleLevel` (see Domain Reorg section below). Left broken intentionally, pending a decision: delete, or repurpose to validate the raw `RoleName` string on incoming DTOs.

---

## Domain / Clean Architecture Reorg — session status (2026-08-12)

Parked mid-pass to switch to another project. Working through
`Inventory-API/Domain/` folder-by-folder to fix placement/DDD violations left
over from the move-everything-into-Domain pass, one file at a time, with
`.claude/RULES.md` (per-subproject architecture rules), `.claude/STACK.md`,
and `/glossary/` bootstrapped at the repo root to support this.

### Done — `Domain/ValueObjects/User/`

- **`EmailConfirmationModel.cs`** — was actually used for password-reset
  emails, not registration confirmation, and was a MediatR integration
  notification (not a sourced domain event — no `UserAggregate` exists).
  Moved to `Services/Service/UserService/Notification/PasswordResetRequestedNotification.cs`,
  renamed, 2 call sites in `UserService.cs` updated. Glossary entry corrected
  to `glossary/inventory-api/password-reset-requested.md`.
- **`UserRole.cs`** — was a bare mutable `{ string? Rolename }`, disconnected
  from the real authorization hierarchy (`RoleLevel`, previously in
  `Shared/Constants/Roles.cs`). `RoleLevel`/`Roles` moved into
  `Domain/ValueObjects/User/RoleLevel.cs` (had to — `Domain` has zero project
  references, so it couldn't reference `Shared` without a circular
  dependency). `UserRole` redesigned as `readonly record struct
  UserRole(RoleLevel Level)`, matching the `Price` value-object pattern.
  Updated 9 consumer files across `Domain`/`Services`/`InventoryApi`
  (`MinimumRoleAttribute`, `UserUtility`, all 5 controllers using
  `[MinimumRole(...)]`, `UserService`, `RoleService`, `UserDetails`). Found
  and fixed 2 inverted-logic bugs along the way (see Roles/User Management
  sections above).
- **`UserWithPassword.cs`** — was `class UserWithPassword : User`, which
  couldn't compile (class inheriting a record) even before the namespace
  collision error masking it (`User` unqualified resolved to the enclosing
  `Domain.ValueObjects.User` namespace, not the entity). Also called a
  `Map()` method that didn't exist anywhere. Redesigned as composition:
  `sealed record UserWithPassword(User User, string PasswordHash)`.
  `UserRepository.GetUser()` now builds the `User` explicitly instead of via
  broken dynamic mapping; `UserService.cs` (`LoginUser`, `GenerateLogin`)
  updated accordingly.

### Not yet started — same folder

`UserAddress.cs`, `UserClaims.cs`, `UserDetails.cs`, `UserId.cs`,
`UserIdentity.cs`, `UserLogin.cs` still need the same placement/design pass.
`UserDetails.cs` and `UserAddress.cs` both still use a `Map(dynamic)`
pattern similar to what `UserWithPassword` had — worth checking whether
they have the same class-of-problem.

### Not yet started — rest of `Domain/`

`Aggregates/`, `Entities/`, `Events/`, and `ValueObjects/Product|Order|Inventory/`
haven't been looked at at all yet.

### Known build blockers (pre-existing, not caused by this session's changes)

- `Domain/Events/Inventory/InventoryEvent.cs` references `InventoryItemIdentity`,
  which no longer exists (shows as deleted in git status) — this is the
  **only** remaining error in `Domain.csproj` as of this session; fixing it
  should unblock a full solution build.
- `NotificationService/Program.cs` has an unrelated C# syntax error.
- `Inventory-DB/InventoryDb/InventoryDb.sqlproj` needs Visual Studio SSDT
  components not available to the `dotnet` CLI — can't build headless.

### Also done this session (setup, not reorg)

- Bootstrapped `.claude/RULES.md`, `.claude/STACK.md`, `glossary/` at the
  repo root via `/project-setup`.
- Removed dead package references: `EntityFramework` (+ unused
  `using System.Data.Entity.Infrastructure;` in 9 files),
  `WindowsAzure.ServiceBus`, `System.Threading.Tasks`, `BCrypt.Net-Core`
  (from `InventoryApi.csproj`), legacy `signalr` npm package (from
  `Inventory-UI`) — all confirmed unused before removal.

### To pick this back up

Resume at `Domain/ValueObjects/User/UserAddress.cs` (or whichever file seems
most useful), or switch to `InventoryItemIdentity` first to unblock a clean
`Domain.csproj` build.
