---
term: Product
scope: supplier-catalogue
services: [Hosted Catalogue (ours), suppliers' own catalogue systems (via adapters)]
avoid: []
related: [Discontinued, Supplier, Product (stock)]
status: draft
---

## Definition
Something a supplier offers to sell, described in the supplier's own terms. Its content,
format and lifecycle are owned by the supplier and differ between suppliers; we receive it
through that supplier's integration and do not define it.

## What we need from every supplier
To order it and later hold it in stock, each supplier must tell us at least: their product id,
what it is (a name), a price, and whether it is still for sale.

## Rules
- Identified by the supplier plus the supplier's own product id; that id is only unique within one supplier.
- Supplier changes affect future orders only; past orders and stock already held are unaffected.
- When a supplier stops selling it, it becomes [Discontinued](discontinued.md).

## Unconfirmed
- The exact minimum set of information (e.g. is pack size required?).
