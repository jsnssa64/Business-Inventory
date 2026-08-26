# Glossary

Ubiquitous language for the Business-Inventory monorepo. Each term is a markdown file with YAML frontmatter (`term`, `scope`, `services`, `aliases`, `related`, `status`) followed by a prose description of the business concept — not the code that implements it.

```
glossary/
├── _template.md          # shape for new entries
├── CONTEXT-MAP.md         # bounded contexts and where their vocabulary overlaps
├── shared/                # terms with the same meaning across multiple projects
├── inventory-api/         # terms specific to the Inventory-API backend
└── notification-service/  # terms specific to NotificationService
```

Status values: `draft` (concept exists in code but is incomplete/evolving), `active` (confirmed, current, safe to reuse), `deprecated` (superseded — kept for history).

This glossary was bootstrapped from a code scan (2026-08-12) as part of `/project-setup`. Several entries carry an inferred-content note — see each entry's closing tag before treating it as settled.
