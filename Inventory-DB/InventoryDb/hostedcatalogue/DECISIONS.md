# Hosted Catalogue (`hostedcatalogue`) decisions

How this context is designed and why. Not a change history; git covers that.

The Hosted Catalogue is our product for suppliers with no online catalogue of their own. We build and run it; the data belongs to each supplier. It's only temporarily in this database (DB-010), and these decisions move with it.

Shared conventions (DB-001 to DB-007, DB-013) and cross-boundary decisions: [`../DECISIONS.md`](../DECISIONS.md). The ones that matter most here are **DB-009** (Inventory reaches this catalogue only through an adapter) and **DB-012** (each system owns its own ids).

---

## HC-001: Own supplier accounts; knows no Registry ids
- **Decision:** the Hosted Catalogue has its own `Supplier` table: the suppliers who use it, as its own accounts. It never stores the Supplier Registry's ids. The Registry stores this catalogue's `SupplierPublicId` as the external id in that supplier's connection details, exactly as it would for a supplier's own system.
- **Why:** it's a standalone product treated like any external catalogue. Depending on Registry ids would couple two services meant to split apart, and would give our own catalogue a shortcut real suppliers' systems don't have.
- **Rejected:** using the Registry's `SupplierPublicId` as this catalogue's tenant key.
- **Consequences:** a supplier on the Hosted Catalogue exists twice, once in the Registry and once here, linked only through connection details. A supplier could use the Hosted Catalogue before being registered on the platform.

## HC-002: Supplier account names are unique forever
- **Decision:** `UQ_HostedCatalogueSupplier_Name` covers all rows, including deactivated accounts. Same rule as the Registry (REG-003).
- **Why:** prevents the same supplier signing up twice by accident, and keeps names unambiguous in support and reporting.
- **Rejected:** non-unique names, with `SupplierPublicId` as the only identity and the name as a display label.
- **Consequences:** suppliers sign up themselves, so a genuine clash (e.g. two sole traders both trading as "Fresh Produce Co") blocks a real customer at sign-up until they choose a distinguishing name. Case-insensitive under the default collation.

## HC-003: Products are identified by ProductPublicId; a SKU always means one physical product
- **Decision:**
  - The id exposed to adapters is `ProductPublicId`, which never changes.
  - The supplier's SKU is an ordinary column, unique per supplier across all products, including discontinued ones (`UQ_HostedCatalogueProduct_SupplierId_Sku`).
  - Pack contents (`ItemsPerPack`, `ItemContentAmount`, `ItemContentUnitCode`; HC-004) can't be changed once a product exists. Selling a different pack size means creating a new product, which gets a new `ProductPublicId` and needs a new SKU.
- **Why:** external suppliers can't be trusted with ids (a SKU may be reused for a changed product). The Hosted Catalogue is our example of a well-behaved supplier system, so it makes the right thing the only option: one SKU, one physical product, forever. Pack contents are a defining fact; if they change, it's a different product.
- **Rejected:** exposing the SKU as the product id (if it's edited or reused, every order line and stocked product pointing at it breaks or points at the wrong product); allowing a SKU to be reused by a new product (the same code would mean two physical products over time).
- **Consequences:** Inventory never relies on our SKUs; the adapter exposes `ProductPublicId`. Suppliers who change a pack size must relabel with a new SKU. "Pack contents and SKU can't change" is enforced by the write procs, which offer no way to change them (HC-007).

## HC-004: Pack contents are always "N items × X of a unit"
- **Decision:** every product describes its pack the same way, as **a pack contains `ItemsPerPack` items, each containing `ItemContentAmount` of `ItemContentUnitCode`**:
  - `ItemsPerPack INT`: whole items only, at least 1.
  - `ItemContentAmount DECIMAL(10,3)`: greater than 0, smallest step 0.001.
  - `ItemContentUnitCode VARCHAR(3)`: a UN/CEFACT Recommendation 20 code (`KGM`, `GRM`, `LTR`, `MLT`, `H87`, ...).

  Totals are never stored. There's no unit lookup table: the code is a snapshot of what the supplier picked from the list the app offers.

  | Product | ItemsPerPack | ItemContentAmount | ItemContentUnitCode |
  |---|---|---|---|
  | bag of flour, 2.4 kg | 1 | 2.4 | `KGM` |
  | case of beer, 6 × 330 ml | 6 | 330 | `MLT` |
  | box of 4 lightbulbs | 4 | 1 | `H87` |
- **Why:**
  - One structure for every product leaves suppliers no room to invent their own descriptions ("6 bottles", "1980 ml", "6 × 330").
  - A total is always calculated (items × content), so it can never disagree with its parts.
  - Stock can count items (a bar counting bottles) or content (a kitchen counting ml) from the same data.
  - `INT` makes fractional item counts impossible by type, with no CHECK needed.
  - Exact decimals, because stock maths on approximate numbers drifts.
  - Scale 3 is enough because the unit carries the scale (finer than 0.001 kg → use `GRM`).
  - The list of units belongs to its source and may grow to thousands of entries. The app (later an external units API) offers it, and the product keeps what was chosen.
  - A standard's codes mean a future units source matches stored values without re-keying, which matters because pack contents can't be changed (HC-003).
- **Rejected:** a single amount + unit (can't describe 6 × 330 ml without losing either the bottle count or the volume); storing a total alongside the parts (they could disagree); `FLOAT` (approximate); a local unit lookup table with an FK (the list would have to be kept in step with its source, and retiring units needs extra handling); our own short codes like `kg`/`each` (would need mapping or migration once a standards-based source arrives).
- **Consequences:** the database doesn't enforce valid unit codes or positive amounts (`ItemsPerPack` at least 1, `ItemContentAmount` greater than 0); the app must, with a test for each (DB-013). Adapters map these codes onto whatever Stock uses. Check the codes against the published UN/CEFACT list before seeding any test data.

## HC-005: List Price is per pack, net of tax, in the supplier's single currency
- **Decision:**
  - `Product.ListPriceAmount DECIMAL(19,4)` is the price of **one pack**, **excluding tax**.
  - The currency is set once per supplier account: `Supplier.CurrencyCode CHAR(3)`, an ISO 4217 code (`EUR`, `GBP`, `USD`) stored as a snapshot string.
  - One price per product, zero or more: `0` is a free item (a sample or promotional item).
- **Why:**
  - Orders are in whole packs, so a per-pack price needs no conversion. A per-kg figure can always be calculated.
  - Business-to-business prices are quoted net; tax rates vary by country and product and change over time, so tax is worked out on the order or invoice, not stored here.
  - Most suppliers price everything in one currency, and one currency per account makes a stray USD price in a GBP catalogue impossible.
  - `DECIMAL(19,4)` is exact, allows sub-cent prices for cheap bulk items, and is the same size (9 bytes) as `DECIMAL(12,2)`.
  - Free items are real catalogue entries a supplier may offer, so `0` is a valid price.
- **Rejected:** a per-unit-of-content price (every order would have to calculate the pack price, with rounding); tax-inclusive prices; currency per product; several prices per product in different currencies; a price that must be greater than 0 (free items would have to go through order negotiation instead); `FLOAT` (approximate); `MONEY` (rounding problems when multiplying and dividing).
- **Consequences:** no price history: changing the List Price overwrites it. Past orders are unaffected, because they freeze their own price. Selling in a second currency would need a separate price table. The app must offer valid ISO 4217 codes and reject negative prices; the database checks neither (DB-013). A forgotten price would look like a free item, so the app must require the price to be entered explicitly (the column has no default).

## HC-006: Discontinued is one nullable date, DiscontinuedAt
- **Decision:** `Product.DiscontinuedAt DATETIME2 NULL`. `NULL` means for sale; a date means discontinued since then. Setting it back to `NULL` is **relisting**: for sale again, as the same product (same `ProductPublicId`). Products are never deleted.
- **Why:** Discontinued is the supplier's decision and can be reversed, so it's a state, not a deletion. The column uses the glossary's term; the glossary avoids "Inactive" and "Deactivated" for it. One column means nothing can disagree, so no `CHECK` is needed (DB-013).
- **Rejected:** `IsDiscontinued BIT` + `DiscontinuedAt` with a `CHECK` (two columns for one fact); reusing DB-004's `DeactivatedAt` (the wrong word for this concept).
- **Consequences:** "for sale" queries use `WHERE DiscontinuedAt IS NULL`. It has the same shape as DB-004's `DeactivatedAt` but a separate name, because it's a different concept.

## HC-007: Products are written only through one proc per intent
- **Decision:** the app writes to `Product` only through stored procedures, one per business action:
  - `AddProduct`: the only place where the SKU and pack contents are set.
  - `UpdateProductDetails`: `Name`, `Description`.
  - `ChangeListPrice`: `ListPriceAmount`.
  - `DiscontinueProduct` / `RelistProduct`: set / clear `DiscontinuedAt` (HC-006).
  - No proc changes the SKU or pack contents, which is how HC-003's lock is enforced.
- **Why:** the rule is enforced by what the procs offer, with no logic in the table (DB-013). Each proc names a business action, matching the event-sourced commands, so "what can change" is readable from the proc list. A future edit history can record the kind of change per proc without diffing old and new values.
- **Rejected:** an `AFTER UPDATE` trigger (logic in the table); `DENY UPDATE` on the columns (only holds with a restricted app login, which doesn't exist yet); C# only (nothing in the database stops a bad write); a single `UpdateProduct` proc (every caller sends every column, and a price change looks like a typo fix).
- **Consequences:** this is only as strong as "nobody writes to the table directly". Until the app connects with a login that can only `EXECUTE` procs, seed scripts, manual fixes and a `dbo` connection can still change pack contents. A new editable column means a new or changed proc, decided per intent.
