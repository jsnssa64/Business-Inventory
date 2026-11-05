# InventoryDb

SQL Server database project (SSDT) for the Business Inventory platform.

> ⚠ **Temporary layout.** This one project currently holds the databases of **three separate services**, as schemas, so they can be built and practised in one place. Each one is built **as if it were already its own service**. The Supplier Registry and Hosted Catalogue schemas will move into their own database projects and repos later. Don't build anything that assumes they'll stay together.

## What lives here

| Schema | Service | Owned by | Status |
|---|---|---|---|
| `org` | Inventory: Identity (businesses, staff) | us | stays here |
| `ordering` | Inventory: Ordering (orders, receipts, frozen prices) | us | stays here |
| `stock` | Inventory: Stock (stocked products, movement ledger) | us | stays here |
| `supplierregistry` | **Supplier Registry**: which suppliers exist, connection details, go-live, which businesses can use them | us (platform operator) | **temporary**: moves to its own service |
| `hostedcatalogue` | **Hosted Catalogue**: products and list prices, for suppliers with no online catalogue of their own | built by us, data owned by each supplier | **temporary**: moves to its own repo |
| `dbo` | legacy model used by the current API | | being replaced by the schemas above |

**Not stored anywhere in this database:**
- Catalogues or prices from suppliers' own systems. Those are read live through one adapter per supplier.
- Per-business prices (Business Price). They belong to a future, separate **Supplier Pricing** service.

The only prices stored here are those frozen on orders.

## Boundary rules

Each schema behaves like a separate service's database:

1. **No foreign keys across schemas.** FKs only within a schema.
2. **Across schemas, store the other side's `PublicId` (GUID)**, never its internal `INT` id.
3. **No procs, views or functions that join or read tables from another schema.** Get the data through the owning service instead.
4. **No transaction that changes tables in more than one schema.** This is impossible once the services are split.
5. **The Hosted Catalogue gets no shortcuts.** Inventory reaches it through an adapter, exactly like any supplier's own catalogue, even though we built it.

Nothing in the build enforces rules 1–4 while everything is in one project, so check for them in review.

## More

- Why the schema looks the way it does: [`DECISIONS.md`](DECISIONS.md) (shared conventions and how contexts relate), plus a `DECISIONS.md` in each schema folder for that context's own decisions
- What the business terms mean: [`/glossary`](../../glossary/README.md), especially `CONTEXT-MAP.md`
