# deploy-scripts

Convenience wrappers for deploying Helm chart components and syncing secrets to Kubernetes, plus a shortcut for bringing up the local Docker Compose stack. Every script is named `deploy.sh` — the folder it lives in says what it deploys.

```
deploy-scripts/
├── docker/deploy.sh            # local Docker Compose stack
└── helm/
    ├── workload/deploy.sh      # application workload chart (api / ui / db / etc.)
    ├── secrets/deploy.sh       # per-service Infisical secret sync (wrapper)
    └── infisical/deploy.sh     # installs the Infisical secrets chart itself
```

## Scripts

### `docker/deploy.sh`

Alias for `Infrastructure/Local/Docker/deploy.sh` — brings up the local Docker Compose stack (creates the three shared networks if they don't already exist, then runs `docker-compose up -d` across all six compose files).

```sh
./docker/deploy.sh
```

No flags — see `Infrastructure/README.md` for the underlying network/service details.

---

### `helm/workload/deploy.sh`

Deploys a Helm chart component (the actual application workload — not secrets) for a given service and environment.

```sh
./helm/workload/deploy.sh -s <service> -e <env> -c <component>
```

| Flag | Required | Default | Description |
|------|----------|---------|-------------|
| `-s` | yes | — | Service name: `inventory-api`, `inventory-db`, `inventory-ui` |
| `-e` | no | `dev` | Environment: `dev`, `staging`, `prod` |
| `-c` | yes | — | Component type: `api`, `sqlserver`, `ui`, `kurrentdb`, `rabbitmq` |
| `-d` | no | off | Dev mode — dry-run + debug preview instead of applying |

Resolves the Helm chart from `Infrastructure/CD/k8s/Components/<component>/` and the values file from `<service>/Chart/env/<env>.values.yaml`.

---

### `helm/secrets/deploy.sh`

Wrapper around `helm/infisical/deploy.sh` for a given service — resolves the service's `Chart/` directory and the Infisical secrets chart path, validates both exist, then hands off to sync Infisical-managed secrets for that service/env/component.

```sh
./helm/secrets/deploy.sh -s <service> -e <env> -c <component>
```

Same flags as `helm/workload/deploy.sh` (minus `-d`).

---

### `helm/infisical/deploy.sh`

The lowest-level of the three Helm scripts — actually installs/upgrades the Infisical secrets chart via Helm (adds the Infisical Helm repo, then `helm upgrade --install` with the merged service + Infisical values files). Called by `helm/secrets/deploy.sh` rather than directly, normally.

```sh
./helm/infisical/deploy.sh -s <service> -e <env> -c <component> -p <serviceChartPath>
```

## Prerequisites

**`helm/workload/deploy.sh` / `helm/secrets/deploy.sh` / `helm/infisical/deploy.sh`:**
- `helm` must be in `PATH`
- Kubernetes context must be set to the target cluster (`kubectl config use-context`)
- Per-environment values files must exist at `<service>/Chart/env/<env>.values.yaml`
- Required Kubernetes secrets (`db-credentials`, `rabbitmq-credentials`, `kurrentdb-credentials`, `security-keys`) must already exist in the target namespace — see `Infrastructure/CD/Platform/secrets/`

**`docker/deploy.sh`:**
- `docker` and `docker-compose` must be in `PATH`
- A `.env` file must exist at `Infrastructure/Local/Docker/.env`
