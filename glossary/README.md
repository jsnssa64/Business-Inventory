# Ubiquitous Language Glossary

This directory is the source of truth for domain terms across all services in this repository. It exists so that everyone — humans and AI assistants alike — uses the same vocabulary when discussing the business, designing APIs, naming database fields, and writing code.

## Why this exists

In a monorepo with multiple services, the same concept can drift into different names across contexts. `Product` in one service might mean the same thing as `Item` in another. `User` and `Customer` might be used interchangeably, or might mean genuinely different things — and nobody is sure which. This glossary makes those decisions explicit and reviewable.

## Structure

```
glossary/
├── README.md                    # This file
├── _template.md                 # Copy this when adding a new term
├── shared/                      # Terms used across multiple services
│   └── <term>.md
├── inventory-service/           # Terms scoped to InventoryService
│   └── <term>.md
├── product-service/             # Terms scoped to ProductService
│   └── <term>.md
└── user-service/                # Terms scoped to UserService
    └── <term>.md
```

**Shared vs scoped:** if more than one service touches a concept, it belongs in `shared/`. If it lives entirely inside one bounded context, it belongs in that service's directory. A term in `shared/` is a contract — changing its meaning affects every service listed in its frontmatter.

## File format

Every term is one markdown file with YAML frontmatter:

```markdown
---
term: InventoryItem
scope: shared                          # "shared" or a service name
services: [InventoryService, ProductService]   # who uses this term
aliases: [Stock Unit, SKU Instance]    # other names this concept has been called
related: [Product, Stock Level]        # adjacent terms worth reading together
status: active                         # active | deprecated | proposed
---

A short, definitive description of what this term means in our domain.

## Notes
Optional: edge cases, common confusions, history of why this term won
over alternatives.
```

The description is the most important field. It must be specific enough that a reviewer can tell whether a newly-proposed term overlaps with this one. Vague descriptions ("a user is a person who uses the system") defeat the purpose.

## Rules for contributing

1. **Search before you add.** Run `/glossary-check <term>` in Claude, or grep this directory, before proposing a new entry. If a similar concept already exists, reuse or extend it instead of creating a parallel term.
2. **One concept per file.** If a term has multiple meanings in different services, that's a smell — split it into scoped entries with clear names.
3. **Aliases are not duplicates.** If `SKU` is what the warehouse team calls an `InventoryItem`, list it in the `aliases:` field of `InventoryItem` rather than creating a separate `SKU` entry.
4. **Deprecate, don't delete.** When a term is retired, set `status: deprecated` and note what replaced it. History matters for understanding old code.
5. **Glossary changes ship via PR.** Same review process as code. Cross-service terms (`scope: shared`) should be reviewed by someone from each affected service.

## How Claude uses this

Claude has a skill (`.claude/skills/ubiquitous-language/`) that teaches it to consult this glossary before suggesting domain names, reviewing PRs, or discussing entity design. Two slash commands are available:

- `/glossary-check <term>` — look up a term, find exact matches, alias matches, and semantic overlaps with existing entries.
- `/glossary-review` — scan the current branch's diff for newly-introduced domain concepts and report which ones already exist in the glossary, which overlap with existing terms, and which are genuinely new.

Claude is configured to be advisory only. It will not create, edit, or delete files in this directory without explicit human instruction.
