# deploy-scripts

Convenience wrappers for deploying Helm chart components and syncing secrets to Kubernetes. These scripts assume they are run from inside the `deploy-scripts/` directory — both scripts `cd ..` to resolve paths relative to the repo root.

## Scripts

### `deploy-component.sh`

Deploys a Helm chart component for a given service and environment.

```sh
./deploy-component.sh -s <service> -e <env> -c <component>
```

| Flag | Required | Default | Description |
|------|----------|---------|-------------|
| `-s` | yes | — | Service name: `inventory-api`, `inventory-db`, `inventory-ui` |
| `-e` | no | `dev` | Environment: `dev`, `staging`, `prod` |
| `-c` | yes | — | Component type: `api`, `sqlserver`, `ui`, `kurrentdb`, `rabbitmq` |

Resolves the Helm chart from `Infrastructure/CD/k8s/Components/<component>/` and the values file from `<service>/Chart/env/<env>-<service>-<component>.values.yaml`.

**Example:**
```sh
./deploy-component.sh -s inventory-api -e dev -c api
```

---

### `deploy-secret.sh`

Deploys Infisical-managed secrets for a service. Delegates to `Infrastructure/CD/Platform/secrets/infisical/deploy.sh` (the actual invocation is currently commented out).

```sh
./deploy-secret.sh -s <service> -e <env> -c <component>
```

Same flags as `deploy-component.sh`. Validates that both the service's `Chart/` directory and the Infisical secrets chart exist before proceeding.

**Example:**
```sh
./deploy-secret.sh -s inventory-api -e dev -c api
```

## Prerequisites

- `helm` must be in `PATH`
- Kubernetes context must be set to the target cluster (`kubectl config use-context`)
- Per-environment values files must exist at `<service>/Chart/env/<env>-<service>-<component>.values.yaml`
- Required Kubernetes secrets (`db-credentials`, `rabbitmq-credentials`, `kurrentdb-credentials`, `security-keys`) must already exist in the target namespace — see `Infrastructure/CD/Platform/secrets/`
