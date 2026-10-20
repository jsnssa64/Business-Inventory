---
term: List Price
scope: supplier-catalogue
services: [Hosted Catalogue (ours), suppliers' own catalogue systems (via adapters)]
avoid: []
related: [Product (supplier-catalogue), Business Price, Order Price]
status: draft
---

## Definition
A supplier's standard price for one of their Products: the price any business is offered
unless the supplier has set a Business Price for it. Set and changed by the supplier.

## Rules
- Always an amount and a currency.
- Changes affect future orders only; orders already placed keep their own Order Price.

## Unconfirmed
- Is it per Product as sold (e.g. per 6-pack) or per unit inside it?
- Does the amount include tax?
- Can a supplier list one Product in more than one currency?
