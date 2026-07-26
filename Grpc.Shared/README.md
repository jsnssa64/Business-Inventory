# Grpc.Shared

Shared gRPC contract library for `AuthenticationService`. Distributed as a NuGet package (`GRPCLibrary`) consumed by services that need to talk gRPC-Authentication.

> **Status: placeholder.** See `TODO.md` — the proto is still the Hello World template.

## Contents

```
Grpc.Shared/
├── Protos/
│   └── authentication.proto   # gRPC service contract
└── Grpc.Shared.csproj
```

## Current Contract

`Protos/authentication.proto` defines `GrpcAuthenticationService` with a single `SayHello(HelloRequest) returns (HelloReply)` RPC — the default gRPC template, not a real authentication contract.

## How It's Consumed

The `.csproj` builds a `GRPCLibrary` NuGet package (versioned via `Version`/`FileVersion`/`AssemblyVersion`). `AuthenticationService` references it via `PackageReference Include="GRPCLibrary"` and includes the proto directly from the installed package path (`$(PkgGRPCLibrary)\content\Protos\authentication.proto`) with `GrpcServices="Server"`.

Distribution today is a manually copied `.nupkg` in `bin/Release/` rather than a project reference or local feed.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Building

```bash
dotnet build
```

## Known Gaps

See `TODO.md`:
- The real authentication contract (methods, request/response messages for admin seeding) needs to be designed before `AuthenticationService` can implement anything.
- `GrpcServices` is set to `"Server"` only, which generates server-side stubs but not client stubs for consumers.
- Package distribution via a manually copied `.nupkg` is fragile.
