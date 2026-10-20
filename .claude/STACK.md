# Tech stack

| Role | Library | Status |
|---|---|---|
| Runtime | .NET 8 | in use |
| API layer | ASP.NET Core + Swashbuckle | in use |
| Command dispatch / CQRS | MediatR 12.5 | in use |
| Event sourcing / event store | KurrentDB (`KurrentDB.Client` 1.0) | next implementation |
| Read / relational persistence | Dapper + Microsoft.Data.SqlClient (SQL project in `Inventory-DB`) | in use |
| Validation | FluentValidation | in use |
| Auth | JWT Bearer, BCrypt.Net-Core | in use |
| Caching | FusionCache | in use |
| Messaging | MassTransit.RabbitMQ (Inventory-API + NotificationService) | in use |
| Inter-service | gRPC (Inventory-API → gRPC service, e.g. AuthService) | planned |
| DI | Microsoft.Extensions.DependencyInjection | in use |

## Open questions
- `EventStore.Client.Grpc.Streams` is still referenced in InventoryApi alongside `KurrentDB.Client` - likely superseded by KurrentDB; remove?
- `EntityFramework` 6.5 (classic EF, not EF Core) referenced in InventoryApi and Services alongside Dapper - in use or leftover?
- NotificationService references both `Confluent.Kafka` and MassTransit/RabbitMQ - intentional or experiment?
