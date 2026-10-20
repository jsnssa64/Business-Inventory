---
term: StockLevel
scope: inventory-service
services: [InventoryService]
aliases: [Quantity on Hand, QoH, Available Count]
related: [InventoryItem, Reservation]
status: active
---

The current count of available units for a given `InventoryItem`, excluding any that are reserved, damaged, or in-transit. StockLevel is a derived value computed from the InventoryItem's state — it is not stored as a standalone field but exposed via the InventoryService API.

## Notes

- This is intentionally scoped to `InventoryService` because the calculation rules (what counts as "available", how reservations are handled) are an internal concern. Other services should ask InventoryService for the StockLevel rather than computing it themselves.
- "Quantity on Hand" (QoH) is warehouse-industry terminology that maps to the same concept and is accepted as an alias.
- Do not confuse StockLevel (a current snapshot) with stock movement events (changes over time). Those are separate.
