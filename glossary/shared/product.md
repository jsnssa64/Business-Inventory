---
term: Product
scope: shared
services: [inventory-api, inventory-ui]
aliases: []
related: [Price, ProductStatus, InventoryItem]
status: draft
---

A distinct item the business sells or stocks. A Product has a name and
description identifying it, a price it's sold at, and a flag for whether
it's currently active — inactive products are presumably hidden or
unavailable for sale, though the code doesn't spell out the consequence.

**Inferred, not confirmed by code:** "presumably hidden or unavailable"
above is a guess at what "inactive" means for the business, not something
stated in the code.

Marked `draft`: whether a product is active is tracked two ways — a
boolean field directly on `Product`, and a separate `ProductStatus` enum
(`Active`/`Inactive`) used in the events that change it. It's unclear
whether these are meant to be the same concept kept in sync, or something
is mid-migration from one to the other.

> **Review before updating:** this entry was generated from an initial code
> scan. Re-check that the description below still reflects the original
> intention before editing it.
