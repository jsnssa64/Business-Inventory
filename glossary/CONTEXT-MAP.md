# Context Map

## Contexts

- **Inventory-API** — Core business logic for managing products, stock, orders, and user accounts; owns the domain model and is the system of record. Maps to: `Inventory-API/`.
- **Inventory-UI** — The interface staff use to view and act on the same business concepts owned by Inventory-API. Maps to: `Inventory-UI/`.
- **NotificationService** — Delivers alerts to connected clients based on events raised elsewhere in the system; doesn't own business concepts of its own. Maps to: `NotificationService/`.
- **AuthenticationService** — Not yet mapped; scan found only infrastructure (gRPC scaffolding), no domain vocabulary to catalogue yet.
- **Inventory-DB** — Not yet mapped; database setup/seeding, no distinct domain vocabulary of its own.

## Overlapping terms

| Term | Contexts | Relationship |
|---|---|---|
| Product | Inventory-API, Inventory-UI | Shared Kernel — same business concept, split across backend/frontend of one product. Defined once in `glossary/shared/product.md`. |
| InventoryItem | Inventory-API, Inventory-UI | Shared Kernel — same concept, though the two representations currently carry different fields (see entry for details). Defined once in `glossary/shared/inventory-item.md`. |
| Order | Inventory-API, Inventory-UI | Shared Kernel — same concept; the backend model is currently far thinner than the `OrderStatus` lifecycle it references. Defined once in `glossary/shared/order.md`. |
| User | Inventory-API, Inventory-UI | Shared Kernel — same concept; the UI's request shapes (registration, password reset, address) are richer than the backend `User` entity currently models. Defined once in `glossary/shared/user.md`. |
