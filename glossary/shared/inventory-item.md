---
term: InventoryItem
scope: shared
services: [InventoryService, ProductService]
aliases: [Stock Unit, SKU Instance, Stock Keeping Unit]
related: [Product, ProductVariant, StockLevel]
status: active
---

A physical, countable instance of a `Product` (or `ProductVariant`) held in inventory. An InventoryItem has a location, a quantity, and a state (available, reserved, damaged, in-transit). Where `Product` answers "what is this thing?", InventoryItem answers "how many do we have and where?"

One Product can have many InventoryItem records — typically one per location or per batch.

## Notes

- The distinction between `Product` and `InventoryItem` is the most commonly violated boundary in this codebase. If you find yourself wanting to put `quantity` on a Product, you want an InventoryItem instead.
- "SKU" is in common usage as an alias here, but technically a SKU is the *identifier code* for a Product variant, not the inventory record itself. We accept the loose usage but the canonical term is InventoryItem.
- `InventoryService` is the only writer for this entity. Other services read via API.
