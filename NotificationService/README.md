# NotificationService

.NET 8 service that consumes inventory events from RabbitMQ (via MassTransit) and pushes real-time notifications to browser clients over SignalR.

> **Status: does not currently compile.** See `TODO.md` — `Program.cs` references undeclared variables and has a hub name mismatch. Fix these before anything else.

## Purpose

Sits downstream of Inventory API: domain events published to RabbitMQ are meant to be consumed here and relayed to connected clients (e.g. the UI) over a SignalR hub at `/hub`.

## Contents

```
NotificationService/
├── Program.cs                          # MassTransit + SignalR host setup (currently broken)
├── Hubs/
│   └── NotificationHub.cs              # SignalR hub — broadcast/single/multi-client sends
├── Consumer/
│   ├── Notification/
│   │   ├── SMSNotificationConsumer.cs  # MassTransit consumer — empty Consume() body
│   │   ├── BatchMessageConsumer.cs     # Not registered in Program.cs — unreachable
│   │   └── Message/NotificationMessage.cs  # No properties defined yet
│   └── BackgroundService/              # Abandoned Kafka consumers — see Known Gaps
└── NotificationService.csproj
```

## Technology Stack

| Concern | Library |
|---|---|
| Framework | ASP.NET Core 8.0 |
| Messaging | MassTransit 8.4 + RabbitMQ |
| Real-time | ASP.NET Core SignalR |

**Consumer config:** `SMSNotificationConsumer` runs with a concurrency limit of 1000 and scheduled redelivery at 5/15/30 minutes on top of an immediate retry.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- RabbitMQ (see `../Infrastructure/Local/Docker/compose.rabbitmq.yml`)

## Getting Started

```bash
dotnet restore
dotnet run
```

Not currently runnable — see Known Gaps below.

## Known Gaps

See `TODO.md` for the full list. Highlights:
- **Won't compile:** `Program.cs` references `buOptions` and `options`, neither of which is declared anywhere.
- **Hub mismatch:** `app.MapHub<ChatHub>("/hub")` references a `ChatHub` class that doesn't exist — the actual hub is `NotificationHub`.
- **Consumer is a no-op:** `SMSNotificationConsumer.Consume` has an empty body; events are received and dropped, never forwarded to SignalR clients.
- **No payload:** `NotificationMessage` has no properties, so there's nothing to carry even once the consumer is implemented.
- **Dead code:** `Consumer/BackgroundService/` contains two duplicate, unused Kafka consumers left over from before the switch to RabbitMQ/MassTransit — slated for removal.
