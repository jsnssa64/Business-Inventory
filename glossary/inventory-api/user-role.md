---
term: UserRole
scope: inventory-api
services: [inventory-api]
aliases: []
related: [User]
status: draft
---

The level of access a User has been granted in the system, used to decide
what they're allowed to do.

**Inferred, not confirmed by code:** elsewhere in this project's
documentation, roles are described as a fixed, ordered hierarchy — Guest
< User < Admin — enforced by a minimum-role check. This value object,
however, only carries a free-form role name string, with no visible link
to that hierarchy. Flagging this as a likely inconsistency between the
documented role model and this value object's current shape, rather than
assuming one is correct.

> **Review before updating:** this entry was generated from an initial code
> scan. Re-check that the description below still reflects the original
> intention before editing it.
