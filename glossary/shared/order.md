---
term: Order
scope: shared
services: [inventory-api, inventory-ui]
aliases: []
related: [OrderStatus, User, Product]
status: draft
---

A customer's request to purchase one or more products, tracked through its
fulfillment lifecycle from being placed through to being shipped,
delivered, cancelled, or refunded.

**Inferred, not confirmed by code:** the description of what an Order
*contains* (which products, quantities, customer) is inferred from general
e-commerce/inventory-management convention — the current `Order` entity
doesn't model any of that yet.

Marked `draft`: the entity itself only carries an identity and a status;
the richer lifecycle (`OrderStatus`) it points to — Pending, Processing,
Shipped, Delivered, Cancelled, Refunded — implies a much fuller concept
(line items, a customer, quantities, a total) that isn't modeled yet.

> **Review before updating:** this entry was generated from an initial code
> scan. Re-check that the description below still reflects the original
> intention before editing it.
