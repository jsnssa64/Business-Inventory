---
term: Business Price
scope: supplier-pricing
services: [Supplier Pricing (separate service, deferred)]
avoid: [Contract Price, Negotiated Price]
related: [List Price, Order Price, Product (supplier-catalogue)]
status: draft
---

## Definition
The price a supplier currently offers one specific business for one of their Products.
It is the [List Price](../supplier-catalogue/list-price.md), unless the supplier has set a different price for that business.
Set and changed by the supplier at any time.

## Rules
- Only that business and that supplier can see it; other businesses never do.
- It is not stored on the business's stock Product; each order takes its own Order Price from it.
- Changes affect future orders only.
