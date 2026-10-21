# Project-Wide TODO

Cross-service concerns, pipelines, and anything that spans more than one directory. Individual service work lives in each service's own `TODO.md`.

Mark done with `[x]` or `[done]`.

---

## CI/CD Pipelines

- [ ] No GitHub Actions workflows exist anywhere in the repo. Build, test, lint, and deploy are entirely manual for every service.
- [ ] `Infrastructure/CI/Terraform/main.tf` is an empty file — cloud infrastructure provisioning not started.

---

## Service-to-Service Connections

### gRPC — AuthenticationService ↔ Inventory-DB

- [ ] Proto contract in `Grpc.Shared` is the Hello World template (`SayHello`), not an authentication contract. Fix this first before implementing `AuthenticationService`.
- [ ] `AuthenticationService` has no implemented RPC methods. The admin seeding pipeline (DB startup → admin user creation) is entirely unbuilt.

### Events — Inventory-API → RabbitMQ → NotificationService

- [ ] `NotificationService` consumer (`SMSNotificationConsumer.Consume`) is an empty body — events published by the API are received and dropped.
- [ ] `NotificationMessage` has no properties, so the consumer can't carry any payload even once implemented.

### Webhooks — Inventory-API → external URLs

- [ ] `IWebhookService.PostWebhookBySubscription` is implemented but never called. Nothing in the API triggers outbound webhook dispatch after any event.

### Email — Inventory-API → email provider

- [ ] `EmailConfirmationModel` is published via MediatR but no handler exists. Forgot-password and password-reset flows emit events into a void.
- [ ] User registration auto-activates accounts as a temporary workaround — the email confirmation gate isn't built.

### Real-time — NotificationService → Inventory-UI via SignalR

- [ ] `NotificationService` hub class name mismatch (`ChatHub` vs `NotificationHub`) prevents hub binding.
- [ ] UI's SignalR components are never mounted anywhere. Even once the service is fixed, the UI won't receive events.

---

## Infrastructure

See `Infrastructure/TODO.md` for Docker Compose issues affecting the local dev stack and the state of the Kubernetes charts.
