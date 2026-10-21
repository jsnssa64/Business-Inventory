# Infrastructure — Claude Guidance

This directory contains all CI and CD infrastructure for the Business Inventory project. Nothing here is application code.

## What lives where

```
Local/Docker/  — Docker Compose for local dev (split by service, merged at startup)
CI/Terraform/  — Terraform stub, not yet in use
CD/k8s/        — Workload Kubernetes manifests and Helm-style component templates
CD/Platform/   — Cluster-level Helm charts (must be installed before workloads)
```

## Local/Docker — Compose split pattern

Compose files are split by service and always merged together via `-f` flags. `compose.network.yml` is always required — it wires the shared networks. The startup order matters: db → kurrentdb → rabbitmq → api → ui.

Three Docker bridge networks must exist before any container starts, one per trust tier:
- `api-shared-network` — UI and API only (the public-facing tier)
- `data-shared-network` — API and DB only (DB is reachable from the host for local tooling, but must not be reachable from the UI)
- `backend-shared-network` — API, RabbitMQ, and KurrentDB (services that must never be reachable except through the API)

`api` is the only service that joins all three networks — it's the sole bridge between tiers. No other service should ever be on more than one network; if it is, that's a sign the trust boundary has been punched through (e.g. UI able to reach DB directly, or DB able to reach RabbitMQ/KurrentDB directly).

If a compose file references a service that isn't on the right network, it will silently lose connectivity rather than fail loudly.

Environment is loaded from `.env` in the same directory. This compose setup is local-dev only — there is no staging/prod `.env` variant.

Always deploy via `Local/Docker/deploy.sh` (aliased at `deploy-scripts/docker/deploy.sh`) rather than invoking `docker-compose` directly — it creates the three networks if they're missing and runs the full `-f` chain in the right order, so it's the one place that has to be kept correct instead of every developer's local invocation drifting independently.

## CD/k8s — Components vs Manifests

`Components/` holds per-service Helm-style templates (they use `{{ }}` syntax and rely on `chart.yaml`/`values.yaml`). `Manifests/` holds the same resources as flat, fully-rendered YAML — useful for `kubectl apply` without Helm.

`Components/unused/` is reference-only (pod, statefulset, cronjob, daemonset, job stubs). Do not apply them.

## CD/Platform — cluster-level charts

These are cluster-wide dependencies, not application workloads. They must be installed first and are environment-aware:

| Chart | Notes |
|---|---|
| `cert-manager/` | Per-environment values under `environments/dev|staging|production` |
| `gateway-api/` | Just CRD installation |
| `nginx-gateway/` | CRDs only (no controller) |
| `nginx-gateway-fabric/` | Full controller chart; `deploy.sh` for install; env overrides under `env/` |
| `secrets/infisical` | Infisical secrets operator |
| `secrets/universal-auth-credentials` | Machine identity for Infisical auth — contains sensitive values, do not log |

## Key constraints to keep in mind

- The three Docker networks must exist before any `docker-compose up` — `deploy.sh` creates them automatically, but a manual `docker-compose up` will hit silent connectivity failures (not startup errors) if run before the networks exist.
- `ENV_MODE` in the compose env is `Dev` (not `Debug` or `Release`) — this controls dacpac build output path in Inventory-DB.
- Platform charts are prerequisites for workloads. If cert-manager or nginx-gateway-fabric is missing, TLS and routing will fail at the cluster level, not per-service.
- Infisical `universal-auth-credentials` chart holds the machine identity secret used to pull secrets at runtime — losing this breaks secret sync for all services.
