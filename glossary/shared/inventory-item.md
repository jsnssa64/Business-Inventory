---
term: InventoryItem
scope: shared
services: [inventory-api, inventory-ui]
aliases: [Stock]
related: [Product]
status: draft
---

The record of how much stock the business currently holds for a given
Product. An InventoryItem tracks a quantity on hand, tied back to the
Product it counts stock for.

**Inferred, not confirmed by code:** the link back to a specific Product is
assumed from context (an inventory count only makes sense in relation to
something being counted); the backend entity itself doesn't carry a
product reference, only its own identity and a quantity.

Marked `draft`: the backend representation is minimal (identity + quantity
only), while the UI's version of this concept also carries the product's
name, description, and price directly — the two representations don't
currently agree on what an InventoryItem is made of.

> **Review before updating:** this entry was generated from an initial code
> scan. Re-check that the description below still reflects the original
> intention before editing it.
