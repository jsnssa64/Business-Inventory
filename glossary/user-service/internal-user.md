---
term: InternalUser
scope: user-service
services: [UserService]
aliases: [Staff, Operator, Admin]
related: [User, Customer]
status: active
---

A `User` whose role grants permissions to operate the system internally — managing inventory, editing the product catalog, viewing operational dashboards, and so on. InternalUsers are created through an administrative provisioning flow, not through public self-signup.

Distinct from `Customer`, who is also a `User` but interacts with the system as a buyer rather than an operator.

## Notes

- "Admin" is in common spoken usage but is technically a *specific* InternalUser role with elevated permissions, not a synonym for all InternalUsers. We accept it as an alias for convenience but be precise in code.
- Authorization checks should generally key off the specific role or permission, not the broad `InternalUser` type. The type is for modelling and conversation, not for access control.
- An identity cannot be both an InternalUser and a Customer in production. (In staging, staff sometimes have shadow Customer accounts for testing.)
