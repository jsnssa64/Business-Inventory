---
term: Product
scope: shared
services: [ProductService, InventoryService]
aliases: [Catalog Item, Listing]
related: [InventoryItem, ProductVariant, SKU]
status: active
---

A sellable concept in our catalog — the abstract thing a customer browses, searches for, and decides to buy. A Product describes *what* something is (name, description, images, category), not *how many of it exist* or *where it physically lives*. Stock and location are properties of `InventoryItem`, not Product.

A single Product may have multiple variants (size, color, configuration) and each variant maps to one or more `InventoryItem` records. The Product itself has no quantity.

## Notes

- Do not use `Product` to mean a physical unit on a shelf. That is an `InventoryItem`.
- A Product can exist without any stock (out-of-stock listings, pre-orders, discontinued items still visible in order history). Treat the relationship to inventory as optional, not mandatory.
- `ProductService` owns the lifecycle of this entity. `InventoryService` references it by ID but does not modify it.
