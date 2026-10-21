# Inventory-DB TODO

Mark done with `[x]` or `[done]`.

---

## Seeding

- [ ] Admin password hash in `Scripts/Post-Deployment/SeedData.sql` is hardcoded. Changing `DEFAULT_ADMIN_PASSWORD` in the environment has no effect — the hash is static.
- [ ] `AuthenticationService` (the gRPC seeder that runs at container startup) has no implementation. See `AuthenticationService/TODO.md`.

---

## Schema

- [ ] `dbo/Table/Password.sql` — `DisabledAt` column defaults to `GETDATE()` for all rows, including active passwords. Should default to `NULL`.
- [ ] `dbo/Table/ServiceSubscription.sql` — missing comma before the FK constraint definition. Schema publish will fail.
- [ ] `dbo/Table/UsersRole.sql` — no index on `(UserId, Role)`. Every role lookup for a user is a full table scan.

---

## Deployment Tracking

- [ ] `_DeployOnce` table uses `nchar(10)` for the filename column. Silent truncation risk for filenames longer than 10 characters.
- [ ] `Scripts/StoredProcedure/TrackFile.sql` — `CATCH` block swallows failures silently. A tracking error allows re-execution of seeding scripts on the next deploy.
