# Glossary

The canonical business vocabulary for this repo. Code implements these definitions,
not the other way round.

## Layout
- `shared/`: terms meaning the same thing across contexts
- `supplier-registry/`: which suppliers exist, go-live, which businesses can use them (ours)
- `supplier-catalogue/`: a supplier's products, list prices, discontinued (supplier-owned; includes our Hosted Catalogue)
- `supplier-pricing/`: business prices (supplier-owned; future service of ours)
- `ordering/`: orders, negotiation, dispatch, receiving
- `stock/`: what a business holds, usage, reorder rules
- `notifications/`: who is told what
- `identity/`: businesses, staff, roles, platform operator
- `CONTEXT-MAP.md`: the contexts and where their vocabulary overlaps
- `_template.md`: the shape every entry follows

## What belongs here
Business concepts a domain expert would use and could misread.
Not attributes (fold them into the parent term), not technical/infrastructure names.
Domain events are covered in each term's Lifecycle section, not as separate entries.

## Avoided names
Each entry's `avoid` field lists names that refer to that concept but shouldn't be used:
incorrect, outdated or misleading (often names still in the code). There is one canonical
name per concept; if you see an `avoid` name, use the canonical term instead.

## Linking terms
In entry text, refer to another term as a link: `glossary term [Discontinued](discontinued.md)`
(relative path, e.g. `../shared/x.md` across folders). `related:` lists canonical term names.

## Status
- `draft`: exists in code but still evolving
- `active`: confirmed and stable; safe to reuse
- `deprecated`: superseded; points to its replacement

## Changing entries
New entries: draft, review, add.
Existing entries: one at a time, discussed before any name/description change.
