# Infrastructure

Local development infrastructure managed with Docker Compose. Compose files are split by service and merged at startup.

## Directory Layout

```
Infrastructure/
├── Local/
│   └── Docker/        # Docker Compose files (one per service)
├── CI/
│   └── Terraform/     # Terraform stub (not yet in use)
└── CD/
    ├── k8s/
    │   ├── Components/    # Per-service Helm-style templates (api, ui, rabbitmq, sqlserver, kurrentdb)
    │   │   └── unused/    # Reference manifests (not deployed)
    │   └── Manifests/     # Flat raw Kubernetes manifests
    └── Platform/          # Cluster-level Helm charts
        ├── cert-manager/
        ├── gateway-api/
        ├── nginx-gateway/
        ├── nginx-gateway-fabric/
        └── secrets/       # Infisical operator + universal-auth-credentials
```

## Docker Networks

Three bridge networks must exist before starting any services — one per trust tier. Create them once:

```bash
docker network create api-shared-network
docker network create data-shared-network
docker network create backend-shared-network
```

- `api-shared-network` — UI and API only (public-facing tier)
- `data-shared-network` — API and DB only (DB also publishes a host port for local tooling, but must not be reachable from the UI)
- `backend-shared-network` — API, RabbitMQ, and KurrentDB (must never be reachable except through the API)

`api` is the only service on all three networks. No other service should be on more than one.

## Services

| File | Service | Ports |
|---|---|---|
| `compose.db.yml` | SQL Server (Inventory-DB) | `${DB_HOST_PORT}:1433` |
| `compose.kurrentdb.yml` | KurrentDB | `2113:2113` |
| `compose.rabbitmq.yml` | RabbitMQ + Management UI | `5672:5672`, `15672:15672` |
| `compose.api.yml` | Inventory API | `3001:8080`, `3002:8081` |
| `compose.ui.yml` | Inventory UI | `3000:3000` |

`compose.network.yml` defines the shared network references and is always required.

## Starting the Full Stack

Run `Infrastructure/Local/Docker/deploy.sh` (or its alias, `deploy-scripts/docker/deploy.sh`) — it creates the three networks if they don't already exist and starts every service:

```bash
./Infrastructure/Local/Docker/deploy.sh
```

Equivalent to running manually:

```bash
cd Infrastructure/Local/Docker

docker-compose \
  -f compose.network.yml \
  -f compose.db.yml \
  -f compose.kurrentdb.yml \
  -f compose.rabbitmq.yml \
  -f compose.api.yml \
  -f compose.ui.yml \
  up -d
```

## Environment Variables

The compose files rely on a `.env` file in the same directory. Required variables:

| Variable | Description | Dev Default |
|---|---|---|
| `MSSQL_ACCEPT_EULA` | SQL Server EULA acceptance | `y` |
| `MSSQL_SA_PASSWORD` | SQL Server SA password | `test123!` |
| `MSSQL_PID` | SQL Server edition | `Developer` |
| `DEFAULT_ADMIN_PASSWORD` | Seeded admin account password | `test123!` |
| `DB_HOST_PORT` | Host port for SQL Server | `1433` |
| `DB_PROJECT_PATH` | Path to Inventory-DB project | _(relative path)_ |
| `API_PROJECT_PATH` | Path to Inventory-API project | _(relative path)_ |
| `UI_PROJECT_PATH` | Path to Inventory-UI project | _(relative path)_ |
| `CONTAINER_NAME` | Suffix for container names | _(e.g. `dev`)_ |
| `ENV_MODE` | Build mode for dacpac | `Dev` |
| `ENVIRONMENT_SHORTFORM` | Environment label used in container naming | `Development` |
| `ASPNETCORE_ENVIRONMENT` | ASP.NET environment | `Development` |
| `RABBITMQ_DEFAULT_USER` | RabbitMQ username | `guest` |
| `RABBITMQ_DEFAULT_PASS` | RabbitMQ password | `test123!` |
| `DB_CONNECTION` | Full SQL Server connection string | — |
| `EVENTSTORE_CONNECTION` | KurrentDB connection string | — |
| `RABBITMQ_CONNECTION` | RabbitMQ connection string | — |
| `SECURITY_ACCESS_KEY` | RSA key for access tokens | — |
| `SECURITY_ACCESS_EXPIRY` | Access token lifetime (minutes) | `20` |
| `SECURITY_REFRESH_KEY` | RSA key for refresh tokens | — |
| `SECURITY_REFRESH_EXPIRY` | Refresh token lifetime (days) | `30` |
| `SECURITY_CONFIRM_KEY` | RSA key for confirmation tokens | — |
| `SECURITY_CONFIRM_EXPIRY` | Confirmation token lifetime | `30` |
| `SECURITY_AUDIENCE` | JWT audience | `http://api.myapp.com` |
| `SECURITY_ISSUER` | JWT issuer | `http://auth.myapp.com` |
| `BASE_URL` | UI base URL | `http://localhost:3000` |
| `NODE_ENVIRONMENT` | Node environment for UI | `Development` |

## Kubernetes (CD)

### Workload — `CD/k8s/`

`Components/` contains per-service Helm-style templates (api, ui, rabbitmq, sqlserver, kurrentdb). `Manifests/` holds the same resources as flat raw manifests. `Components/unused/` is reference-only and not deployed.

### Platform — `CD/Platform/`

Cluster-level charts that must be installed before workloads:

| Chart | Purpose |
|---|---|
| `cert-manager/` | TLS certificate management; per-environment values under `environments/` |
| `gateway-api/` | Gateway API CRD installation |
| `nginx-gateway/` | NGINX Gateway CRDs |
| `nginx-gateway-fabric/` | NGINX Gateway Fabric controller; environment overrides under `env/`; `deploy.sh` for install |
| `secrets/infisical` | Infisical secrets operator |
| `secrets/universal-auth-credentials` | Machine identity credentials for Infisical auth |

### Terraform — `CI/Terraform/`

Stub only — not yet in use.
