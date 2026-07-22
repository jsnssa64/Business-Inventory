# Inventory DB

SQL Server 2022 database container. The database schema is deployed via a DACPAC built from the `InventoryDb` project. An `AuthenticationService` runs alongside SQL Server inside the container to seed the initial admin account.

## Directory Layout

```
Inventory-DB/
├── InventoryDb/            # SQL Server Database project (.sqlproj)
├── AuthenticationService/  # .NET service that seeds the default admin login
├── Grpc.Shared/            # Shared gRPC contracts
├── server/
│   ├── entrypoint.sh       # Container startup script
│   └── CreateServerLogin.sql
└── Dockerfile
```

## How It Works

1. The Dockerfile builds on `mcr.microsoft.com/mssql/server:2022-latest`.
2. It installs `sqlpackage` and copies the compiled `InventoryDb.dacpac` and `master.dacpac` from `./bin/${BIN_MODE}/` into the image.
3. On container start, `entrypoint.sh` starts SQL Server, waits for it to be ready, applies the dacpac to create/migrate the schema, and runs `CreateServerLogin.sql` to create the service account.
4. `AuthenticationService` seeds the default admin user using the `DEFAULT_ADMIN_PASSWORD` environment variable.

## Building

The DACPAC must be compiled before building the Docker image. Build the `InventoryDb` SQL project first (requires the SQL Server Data Tools or `dotnet build` with the appropriate SDK), then:

```bash
# From the repo root
docker build \
  --build-arg BIN_MODE=Debug \
  -t inventory-db \
  ./Inventory-DB
```

`BIN_MODE` defaults to `Debug`. Use `Release` for production images.

## Environment Variables

| Variable | Description |
|---|---|
| `ACCEPT_EULA` | Must be `y` to accept the SQL Server EULA |
| `MSSQL_SA_PASSWORD` | SA account password |
| `MSSQL_PID` | SQL Server edition (e.g. `Developer`, `Express`) |
| `ASPNETCORE_ENVIRONMENT` | Passed through to the seeding service |
| `DEFAULT_ADMIN_PASSWORD` | Password for the seeded admin account |

## Running via Docker Compose

See `../Infrastructure/README.md`. The `compose.db.yml` file handles all variables via the shared `.env` file.

## Connecting Locally

When running via Docker Compose the default host port is controlled by `DB_HOST_PORT` (default `1433`).

Connection string for local API development:

```
Server=localhost,1433;Database=InventoryDb;User Id=defaultAdmin;Password=<DEFAULT_ADMIN_PASSWORD>;TrustServerCertificate=True
```
