---
term: Order Price
scope: ordering
services: [Inventory-API]
avoid: []
related: [Business Price, List Price, Order]
status: draft
---

## Definition
The price of something on one order. It has two moments:
- **Expected price**: the [Business Price](../supplier-pricing/business-price.md) at the moment the order is submitted.
- **Agreed price**: the price once the supplier confirms the order, including any discount or
  change the supplier applied to a line or to the whole order.

## Rules
- Once agreed, it never changes, whatever happens to the supplier's prices afterwards.
- A supplier's change to the price must be agreed by the business before it becomes the agreed price.
- On an order the business marked as fixed, the supplier can't change it; agreed = expected.
- The same Product bought on different orders can have different Order Prices.

## Unconfirmed
- A whole-order discount: is it spread across the lines (for "what did each item cost us?"), or kept as one amount?
