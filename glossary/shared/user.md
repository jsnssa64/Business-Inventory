---
term: User
scope: shared
services: [inventory-api, inventory-ui]
aliases: [Account]
related: [UserRole, UserClaims, UserAddress, EmailConfirmation, Order]
status: draft
---

A person who has an account in the system and can sign in to use it. Every
User has a username and email that identify them, and is assigned a role
that determines what they're permitted to do — for example, whether they
can only view inventory or also manage products and orders.

**Inferred, not confirmed by code:** the description above assumes Users
are the staff/operators of the business (people managing inventory,
products, and orders) rather than end customers, based on the overall
shape of the system. The code itself only defines username, email, and
role — it doesn't say who these people are in the business.

Marked `draft`: the core `User` entity is a bare 3-field record, while the
UI's request shapes for registration, password reset, and profile updates
(name, address, phone, date of birth) are considerably richer — suggesting
the full concept of a User isn't yet settled or fully reflected in the
backend entity.

> **Review before updating:** this entry was generated from an initial code
> scan. Re-check that the description below still reflects the original
> intention before editing it.
