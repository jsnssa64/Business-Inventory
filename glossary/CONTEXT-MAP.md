# Context Map

Purpose: a multi-business inventory and ordering platform: businesses track the stock they hold
and buy more from a curated network of integrated suppliers, negotiating price per order.
Principle: business-first. We model what a business holds, not what a supplier holds.

## Contexts
- **Supplier Registry** (ours, separate service): which suppliers exist on the platform, how to
  connect to them, when they go live, and which businesses can use them. Run by the platform
  operator. Platform-wide. Does **not** store catalogues or prices.
- **Supplier Catalogue** (owned by each supplier): what a supplier sells and its List Price.
  Lives in the supplier's own system (an API, or even a spreadsheet) and is read live through one
  adapter per supplier; never stored by us. The **Hosted Catalogue** is our product for suppliers
  with no online catalogue: built and run by us, data owned by the supplier, reached through an
  adapter like any other catalogue.
- **Supplier Pricing** (ours, separate service, deferred; data owned by each supplier): the
  Business Price a supplier sets for one business. Works with any catalogue.
- **Ordering**: buying from suppliers: orders, price negotiation, dispatch, and recording
  what was received. Per business; a supplier sees only orders addressed to it.
- **Stock**: what a business physically holds: stocked items, quantities, usage,
  reorder rules, low-stock warnings. Per business.
- **Notifications**: telling the right people when something happens, per business preferences.
- **Identity**: businesses, their staff and roles, the platform operator, supplier access. Platform-wide.

## Relationships
| Upstream → Downstream | What flows | Relationship |
|---|---|---|
| Supplier Registry → Ordering | which suppliers a business can use, and how to reach them | Customer/Supplier |
| Supplier Catalogue → Ordering | Products and List Prices, read live | Anticorruption Layer (one adapter per supplier) |
| Supplier Pricing → Ordering | Business Prices (deferred) | Customer/Supplier |
| Ordering → Stock | received quantities | Customer/Supplier |
| Ordering, Stock → Notifications | things that happened (order amended, low stock…) | Customer/Supplier |
| Identity → all | who is acting, for which business | Conformist (all contexts accept Identity's model) |

## Overlapping terms
| Term | Contexts | Relationship |
|---|---|---|
| Product | Supplier Catalogue, Stock | Same business word, separate meanings: the supplier's offering (supplier-owned, varies per supplier) vs what the business holds (copied when bought). Stock translates the supplier's Product into its own through an Anticorruption Layer (one adapter per supplier). See [supplier-catalogue/product.md](supplier-catalogue/product.md) and [stock/product.md](stock/product.md). |
| Price | Supplier Catalogue, Supplier Pricing, Ordering | Not one term: split into [List Price](supplier-catalogue/list-price.md) (catalogue), [Business Price](supplier-pricing/business-price.md) (per business; deferred), both current and supplier-owned, and [Order Price](ordering/order-price.md) (frozen per order). Price offered = Business Price if set, else List Price; Ordering snapshots it when an order is submitted, and nothing is synced back. |

## Avoided names
- **Supplier Network**: an outdated umbrella name. It mixed up the Supplier Registry, the Supplier Catalogue and Supplier Pricing, and implied we store catalogues. Use the specific context name.
