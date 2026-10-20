---
term: Product
scope: stock
services: [Inventory-API]
avoid: []
related: [Product (supplier-catalogue), Discontinued]
status: draft
---

## Definition
Something a business physically holds in stock. It starts as a copy of what the supplier
described when it was bought, and from then on belongs to the business: changes on the
supplier's side don't alter it. Each Product records how many the business currently has.

## Lifecycle
First received from a supplier → more received / used by the business → runs out.

## Rules
- Identified by the business, the supplier, and the supplier's product id. The same real-world
  item bought from two suppliers is two Products.
- Stock goes up only when the business records a delivery as received, and down when the business uses it.
- If the supplier's Product is [Discontinued](../supplier-catalogue/discontinued.md), what's held can
  still be used, but no more can be ordered from that supplier.

## Unconfirmed
- Is the copy taken once (first purchase) or refreshed on every delivery?
- Is stock counted in packs or individual units?
- What happens to a Product once it reaches zero and can't be reordered: kept for history, or archived?
