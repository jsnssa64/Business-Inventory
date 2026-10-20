# NotificationService TODO

Mark done with `[x]` or `[done]`.

---

## Won't Compile — Fix First

- [ ] `Program.cs` references variables (`buOptions`, `options`) that are never declared. The service does not currently compile.
- [ ] Hub is mapped as `ChatHub` but the class is named `NotificationHub` — fix once compile errors are resolved.
- [ ] `cfg.ConfigureEndpoints(context)` is called twice in the RabbitMQ setup block — remove the duplicate.

---

## Consumer

- [ ] `SMSNotificationConsumer.Consume` is an empty method body. Events received from RabbitMQ are logged and dropped — nothing is pushed to SignalR clients.
- [ ] `NotificationMessage` has no properties. Even once the consumer is implemented, no payload can be carried or acted on.
- [ ] `BatchMessageConsumer` is not registered in `Program.cs` — unreachable.

---

## Abandoned Code to Remove

- [ ] `KafkaConsumerService.cs` and `ConsumerService.cs` are duplicate Kafka background services. The project uses RabbitMQ/MassTransit. Both files should be deleted.
