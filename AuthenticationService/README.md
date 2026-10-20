# AuthenticationService

.NET 8 gRPC service intended to seed the initial admin user into `Inventory-DB` at container startup.

> **Status: scaffolded only.** See `TODO.md` — no RPC methods are implemented yet.

## Purpose

Runs alongside SQL Server in the `Inventory-DB` container (see `../Inventory-DB/README.md`) and seeds the default admin login using the `DEFAULT_ADMIN_PASSWORD` environment variable, over gRPC rather than direct DB access.

## Contents

```
AuthenticationService/
├── Program.cs               # Minimal gRPC host setup
├── Services/
│   └── AuthenticationService.cs   # gRPC service impl — no methods implemented
├── appsettings.json / appsettings.Development.json
└── AuthService.csproj
```

## Technology Stack

| Concern | Library |
|---|---|
| Framework | ASP.NET Core 8.0 (gRPC) |
| Contract | `Grpc.Shared` (`GRPCLibrary` NuGet package) |
| Protobuf | Google.Protobuf, Grpc.AspNetCore |

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- The `Grpc.Shared` proto contract (currently a Hello World placeholder — see `../Grpc.Shared/README.md`)

## Getting Started

```bash
dotnet restore
dotnet run
```

gRPC calls require a gRPC client — the root HTTP endpoint (`/`) just returns a pointer to the gRPC client docs.

## Known Gaps

See `TODO.md`:
- `Grpc.Shared`'s proto contract needs to be designed for real authentication/seeding RPCs before this service can implement anything (currently inherits the Hello World `SayHello` template).
- `Services/AuthenticationService.cs` overrides no RPC methods — any call throws unimplemented.
- `AuthService.csproj`'s `DockerfileContext` incorrectly points at `..\InventoryDb` instead of this service's own directory.
