---
term: Discontinued
scope: supplier-catalogue
services: [Hosted Catalogue (ours), suppliers' own catalogue systems (via adapters)]
avoid: [Inactive, Deactivated]
related: [Product (supplier-catalogue), Product (stock)]
status: draft
---

## Definition
A supplier's Product that the supplier no longer sells. It is the supplier's decision,
and they can reverse it.

## Rules
- It stays in the supplier's catalogue, marked as no longer for sale; it is never removed.
- If the supplier starts selling it again, they **relist** it: it is for sale again as the same Product.
  Relisting only ends Discontinued; the Product never left the catalogue.

What other contexts do when a Product is Discontinued is described there:
[Product (stock)](../stock/product.md), and Ordering's Order entry (not yet drafted).
