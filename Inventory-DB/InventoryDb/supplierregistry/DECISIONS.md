# Supplier Registry (`supplierregistry`) decisions

How this context is designed and why. Not a change history; git covers that.

The Supplier Registry is a separate service of ours, only temporarily in this database (DB-010). These decisions move with it.

Shared conventions (DB-001 to DB-007, DB-013) and cross-boundary decisions: [`../DECISIONS.md`](../DECISIONS.md). The ones that matter most here are **DB-008** (businesses are referenced by `BusinessPublicId`), **DB-009** (the Registry doesn't store catalogues or prices), **DB-011** (Business Price belongs elsewhere) and **DB-012** (the Registry's own supplier ids; external ids in connection details).

---

## REG-001: Suppliers are public unless explicitly restricted
- **Decision:** `Supplier.IsRestricted BIT DEFAULT 0`. `0` means visible to every business. `1` means visible only to the businesses listed in `SupplierAccess` (many-to-many). A restricted supplier with no access rows is visible to nobody.
- **Why:** a business may bring its own private supplier, and more businesses can be granted access to it later by adding rows. The explicit flag makes the model fail closed.
- **Rejected:** "no access rows = public". Deleting the last access row would silently expose a private supplier to every business, breaking supplier isolation. Also rejected: a single nullable `OwnerBusinessId`, which allows only one business per private supplier.
- **Consequences:** every query that lists suppliers for a business must check `IsRestricted` and, when it's set, `SupplierAccess`. This needs an explicit test, like the other isolation rules.

## REG-002: Readiness is separate from restriction: LiveAt
- **Decision:** `Supplier.LiveAt DATETIME2 NULL`. `NULL` means still onboarding and visible to nobody; a date means live since then. A supplier is visible to a business only when it's live, active, and either not restricted or listed in `SupplierAccess` for that business.
- **Why:** registering a supplier doesn't mean it's ready. Integration and setup can happen after the record exists. "Who can see it" (`IsRestricted`) and "can anyone see it yet" (`LiveAt`) are separate questions.
- **Rejected:** defaulting `IsRestricted` to 1 to mean "not ready". A private supplier being onboarded would become visible to its business as soon as an access row was added. And "restricted with no rows" would mean either "private" or "onboarding", with no way to tell which.
- **Consequences:** going live is an explicit step (set `LiveAt`). Supplier visibility queries check three things; test each one.

## REG-003: Supplier names are unique forever
- **Decision:** `UQ_SupplierRegistrySupplier_Name` covers all rows, including deactivated suppliers. Same rule as business names (ID-001).
- **Consequences:** two real suppliers with the same trading name can't both be registered as-is; one needs a distinguishing name. Case-insensitive under the default collation.

## REG-004: SupplierAccess is a bare pairing with a composite key
- **Decision:** `SupplierAccess` holds only `SupplierId` (FK) + `BusinessPublicId` (a plain value, no cross-schema FK; DB-008), with PK `(SupplierId, BusinessPublicId)` and an index on `BusinessPublicId`. There's no surrogate Id, PublicId, audit or lifecycle columns.
- **Why:** a join table isn't an entity. Nothing references a row, and the pairing is its identity. The composite PK also stops the same business being granted twice. The PK serves "who can see supplier X"; the `BusinessPublicId` index serves "which suppliers can business Y see".
- **Rejected:** recording who granted access and when.
- **Consequences:** revoking access deletes the row, so there's no record of past grants. Add `GrantedAt`/`GrantedBy` (or soft revoke) if access ever needs auditing.

## REG-005: No business opt-in to suppliers
- **Decision:** businesses don't opt in to suppliers. The platform sets up each supplier, decides when it goes live (`LiveAt`), and decides who can see it (`IsRestricted` + `SupplierAccess`). A business that wants a new supplier asks the platform, which signs the supplier up.
- **Why:** supplier onboarding is a platform responsibility, not a business choice. A separate opt-in table would duplicate the supplier + business pairing in `SupplierAccess` with a different meaning.
- **Rejected:** a business opt-in table (`BusinessSupplier`); merging opt-in into `SupplierAccess`.
- **Consequences:** every live, public supplier is available to every business. If businesses ever need to hide or favourite suppliers, that's a business preference to model in Inventory, not here.
