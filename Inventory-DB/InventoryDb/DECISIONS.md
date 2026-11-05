# InventoryDb decisions: shared

How the database is designed and why. Each entry gives the decision, why it was made, the alternatives rejected, and its consequences.

This is **not a change history**; git covers that. When a decision changes, rewrite its entry, and put the reason for the change in the commit message.

This file holds **conventions** for every schema and decisions about **how contexts relate**. Decisions that belong to one context live in that schema's folder:
- [`org/DECISIONS.md`](org/DECISIONS.md): Identity (`ID-`)
- [`supplierregistry/DECISIONS.md`](supplierregistry/DECISIONS.md): Supplier Registry (`REG-`)
- [`hostedcatalogue/DECISIONS.md`](hostedcatalogue/DECISIONS.md): Hosted Catalogue (`HC-`)

---

## Conventions

## DB-001: Internal INT key + public GUID
- **Decision:** every entity gets `<Table>Id INT IDENTITY` as its PK and `<Table>PublicId UNIQUEIDENTIFIER DEFAULT NEWID()` (UNIQUE) for anything exposed outside the DB.
- **Why:** only one place creates each id, so an INT is enough internally: small, sequential, fast joins. The GUID stops outsiders (e.g. a supplier contacting a business) from seeing or guessing sequential ids, or inferring how many customers we have.
- **Rejected:** INT only (leaks ids); GUID as the PK (wider keys and indexes, no need yet).
- **Consequences:** APIs and integrations use the PublicId; joins use the Id. PublicId always has a UNIQUE constraint, so lookups by it are indexed. Join tables are the exception: they use a composite PK of the ids they pair.

## DB-002: Name every constraint
- **Decision:** PK, UQ, FK, CK and DF constraints are always named explicitly (`PK_Business`, `UQ_Business_Name`, ...). They're declared at table level, except defaults.
- **Why:** errors show a readable name instead of `UQ__Business__737584F6`. Names are the same in every database, so hand-written scripts (`DROP CONSTRAINT ...`) work everywhere.
- **Rejected:** unnamed constraints, which get a random system name that differs per database.
- **Consequences:** table-level placement is a style choice. Inline-but-named would also work, but multi-column constraints have to be table-level anyway.

## DB-003: DATETIME2 in UTC
- **Decision:** time columns are `DATETIME2`, stored as UTC, defaulting to `SYSUTCDATETIME()`.
- **Why:** `DATETIME` is legacy, with ~3 ms rounding that breaks round-trips with C# `DateTime`. UTC avoids the Docker (UTC) vs Windows (local) mismatch you'd get with `GETDATE()`.
- **Rejected:** `DATETIME`/`GETDATE()` (legacy, local time); `DATETIMEOFFSET` (no need for each business's local time yet).
- **Consequences:** the app converts to local time for display. Revisit `DATETIMEOFFSET` if a business's local time ever matters, e.g. "delivered at 9am Lisbon time".

## DB-004: Lifecycle is one nullable date, DeactivatedAt
- **Decision:** soft delete via `DeactivatedAt DATETIME2 NULL` only. `NULL` means active; a date means deactivated since then. There is no `IsActive` column.
- **Why:** rows are deactivated, never deleted, because other tables reference them. One column holds one fact, so nothing can contradict it and no `CHECK` is needed (DB-013). HC-006 uses the same shape for `DiscontinuedAt`, so a nullable date reads as "live while NULL" everywhere.
- **Rejected:**
  - `IsActive BIT` + `DeactivatedAt` with a `CHECK` keeping them in sync. It stores one fact twice, and the `CHECK` exists only to guard the duplicate.
  - Both columns without the `CHECK`. Any write that bypasses the C# can leave them disagreeing.
  - `IsActive` as a computed column from `DeactivatedAt`. It adds nothing over a C# property.
- **Consequences:** active queries use `WHERE DeactivatedAt IS NULL`; anyone reading the raw tables must know that convention. The C# models expose `IsActive => DeactivatedAt is null` for readability.

## DB-005: Audit columns CreatedAt + UpdatedAt
- **Decision:** `CreatedAt` (NOT NULL, default `SYSUTCDATETIME()`) and `UpdatedAt` (NULL until the first change).
- **Why:** basic traceability of when a row was created or last changed.
- **Consequences:** nothing sets `UpdatedAt` automatically; update procs (or a trigger) must set it.

## DB-006: Index foreign key columns
- **Decision:** FK columns get a nonclustered index, e.g. `IX_StaffMember_BusinessId`. So do columns holding another context's PublicId that are queried by it, e.g. `IX_SupplierAccess_BusinessPublicId`.
- **Why:** SQL Server doesn't index FKs automatically, and "all X for business Y" is the most common query shape in a multi-tenant DB.
- **Consequences:** slightly slower writes, which is negligible at this scale.

## DB-007: Schema names match their service; constraints on same-named tables are prefixed
- **Decision:** each schema is named after the service it belongs to (`supplierregistry`, `hostedcatalogue`), never a generic word like `supplier`. When two schemas have a table with the same name (e.g. `Supplier`), its constraints are prefixed with schema + table: `PK_SupplierRegistrySupplier`, `PK_HostedCatalogueSupplier`.
- **Why:** a generic name like `supplier` reads as "everything about suppliers" and hides which service owns it. Constraint names are only unique per schema, and error messages show the name without the schema, so `UQ_Supplier_Name` wouldn't say which table failed.
- **Rejected:** generic schema names; renaming same-named tables instead (e.g. `hostedcatalogue.SupplierAccount`).
- **Consequences:** tables with unique names keep plain constraint names (`PK_SupplierAccess`).

## DB-013: Tables hold the shape of the data, not business logic
- **Decision:** constraints describe what a valid row *is*: types, `NOT NULL`, PK, FK, `UNIQUE`. Business rules and validation live in the C# code. A `CHECK` constraint is added only when chosen deliberately for that case, never by default. Conditional rules (e.g. "whole numbers only for count units") never go in a `CHECK`.
- **Why:** business rules are easier to read, test and change in C#. Logic spread across tables is hard to find and easy to forget. Shape constraints still guard against writes that bypass the C#, such as seed scripts, manual fixes, or other services.
- **Rejected:** enforcing business rules in the database by default.
- **Consequences:** the C# validation needs tests for every rule the database doesn't enforce. There are currently no `CHECK`s.

---

## How contexts relate

## DB-008: Cross-schema references use the PublicId
- **Decision:** when a table refers to an entity owned by another schema, it stores that entity's `<Table>PublicId` (GUID), never its internal `<Table>Id`, and has no FK to it. Example: `supplierregistry.SupplierAccess.BusinessPublicId`.
- **Why:** each schema stands for a separate context, and some are separate services that are only temporarily in this DB (DB-010). The internal INT is a private join key (DB-001); stored in another context, it would leak that key across the boundary once the services are split. Storing an id is how contexts are meant to refer to each other. FKs, reading another schema's tables, or copying its data would be crossing the boundary.
- **Rejected:** storing the internal INT because the schemas currently share one DB.
- **Consequences:** GUID columns (16 bytes) on cross-schema references and their indexes. Inside a schema, joins still use the INT.

## DB-009: We don't store supplier catalogues or prices
- **Decision:** catalogues (products + List Prices) stay in each supplier's own system and are read live through one adapter per supplier. The Supplier Registry holds only which suppliers exist, their connection details, when they go live, and which businesses can use them. The only prices we store are frozen on orders: `OrderLine.ExpectedUnitPrice`, `OrderLine.AgreedUnitPrice`, `Order.OrderDiscountAmount`.
- **Why:** suppliers should need as little as possible to join; a catalogue could be a spreadsheet, with no API. Adapters absorb the differences. Storing catalogues would mean keeping copies in sync and owning data that isn't ours. Orders snapshot product and price, so nothing depends on the catalogue staying the same after ordering.
- **Rejected:** storing catalogues and prices ourselves (e.g. `Product` and `BusinessPrice` tables in the Registry).
- **Consequences:**
  - A supplier's catalogue is unavailable while their system or file is unavailable.
  - Supplier connection details stay on our backend; businesses never see credentials.
  - The units-of-measure lookup belongs to `stock` alone.
  - Our own Hosted Catalogue is one of these catalogues: reached through an adapter, never read directly (HC-001).

## DB-010: Three services' databases share one project for now
- **Decision:** Inventory (`org`, `ordering`, `stock`), the Supplier Registry (`supplierregistry`) and the Hosted Catalogue (`hostedcatalogue`) live as schemas in this one `.sqlproj` and database. This is temporary. The Registry and Hosted Catalogue will move into their own database projects, then their own repos and services.
- **Why:** SQL practice. One project, one DB and one Docker deploy keep the setup simple while the schemas are designed.
- **Rejected (for now):** one `.sqlproj` per service. It's better for the end goal, because SSDT would fail the build (SQL71501) on any cross-service reference, but it means more projects and a Docker deploy of several DACPACs.
- **Consequences:** boundaries are kept by discipline only. See the rules in [`README.md`](README.md), and check them in review. Each context keeps its decisions in its own folder so they move with it.

## DB-011: Business Price belongs to its own future service, Supplier Pricing
- **Decision:** per-business prices (Business Price) belong in a separate service, Supplier Pricing, that's ours to build, with the data owned by suppliers. It's keyed by supplier + the supplier's own product id. It's deferred: for now, the price offered is the List Price, and special deals happen through order negotiation.
- **Why:** many suppliers expose a catalogue and list prices but have no business-price endpoint, so someone has to hold those prices. They're needed whether the catalogue is the supplier's own or our Hosted Catalogue, so they can't be tied to either. They're supplier-owned commercial data, so they don't belong in the operator-owned Registry. One service also means one place to design and test price isolation.
- **Rejected:** inside the Hosted Catalogue (a supplier with its own catalogue API would have to adopt ours just for pricing); inside the Supplier Registry (mixes operator-owned and supplier-owned data, needs supplier logins, and puts the most sensitive data in a service built for something else).
- **Consequences:** price offered = Business Price ?? List Price, worked out before the order is submitted and frozen on the order line. Ordering doesn't care where the price came from. No Business Price tables in any schema until this service is designed.

## DB-012: Each system owns its own ids; external ids live in connection details
- **Decision:** the Supplier Registry assigns every supplier its own `SupplierPublicId`, which suppliers never see. The id a supplier has in its own system (or in the Hosted Catalogue) is stored only as part of that supplier's connection details. This applies to every supplier, including those on our Hosted Catalogue.
- **Why:** it keeps the services decoupled. Nothing has to be kept in step across services, and a supplier can change systems (Excel → API → Hosted Catalogue) by changing only its connection details. External ids from different suppliers can collide (A's "1" and B's "1"); they're never used alone, always scoped by the Registry supplier, the same way `SupplierProductId` is only unique within one supplier.
- **Rejected:** sharing one supplier id across systems.
- **Consequences:** credentials in connection details must be stored as a reference to a secret store, never in plain columns (to design with connection details). See HC-001 for the Hosted Catalogue side.
