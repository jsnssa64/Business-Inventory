---
term: User
scope: shared
services: [UserService, ProductService, InventoryService]
aliases: [Account]
related: [InternalUser, Customer]
status: active
---

Any authenticated identity in the system, regardless of role or purpose. A User has credentials, a unique ID, and an audit trail. Both internal staff operating the inventory and external customers browsing products are Users.

Where role-specific behaviour matters, use the more specific terms `InternalUser` (staff with operational permissions) or `Customer` (external user with purchasing intent). Code that genuinely does not care about the distinction can use `User`.

## Notes

- Avoid using `User` when you specifically mean a customer or specifically mean staff. The ambiguity has caused permission bugs in the past.
- `Account` is accepted as an alias but is being phased out in new code to avoid confusion with billing-style account concepts.
- `UserService` owns identity, authentication, and the role assignment that distinguishes `InternalUser` from `Customer`.
