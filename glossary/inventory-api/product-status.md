---
term: ProductStatus
scope: inventory-api
services: [inventory-api]
aliases: []
related: [Product]
status: draft
---

Whether a Product is currently active (available/visible) or inactive
(withdrawn) — the state changes that Product lifecycle events like
`ProductActivated`/`ProductDeactivated` record.

Marked `draft`: this exists as a separate enum from the boolean `Active`
flag already on the `Product` entity — see the open question raised in the
`Product` entry about whether these represent the same fact tracked twice.

> **Review before updating:** this entry was generated from an initial code
> scan. Re-check that the description below still reflects the original
> intention before editing it.
