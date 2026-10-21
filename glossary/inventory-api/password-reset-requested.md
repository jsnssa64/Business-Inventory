---
term: PasswordResetRequested
scope: inventory-api
services: [inventory-api]
aliases: [EmailConfirmationModel, PasswordResetRequestedNotification]
related: [User]
status: draft
---

The moment a User asks to reset a forgotten password (by email or by
username), which should result in them being emailed a one-time token they
can use to set a new password.

Marked `draft`: as it stands, nothing actually consumes this and sends the
email — there's no handler wired up for it, so a password-reset request
currently has no visible effect for the user.

This is an application-layer notification (MediatR, in-process fan-out),
not a sourced domain event — it isn't raised by an aggregate and isn't
persisted to the event store the way `ProductActivated`/`OrderCreated`
are. It exists to decouple "a reset was requested" from "an email got
sent," not to record a permanent fact about a User.

> **Review before updating:** this entry was generated from an initial code
> scan. Re-check that the description below still reflects the original
> intention before editing it.
