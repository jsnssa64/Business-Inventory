# Infrastructure — Claude Guidance

This directory contains all CI and CD infrastructure for the Business Inventory project. Nothing here is application code.

## What lives where

```
CI/Docker/     — Docker Compose for local dev (split by service, merged at startup)
CI/Terraform/  — Terraform stub, not yet in use
CD/k8s/        — Workload Kubernetes manifests and Helm-style component templates
CD/Platform/   — Cluster-level Helm charts (must be installed before workloads)
```

## CI/Docker — Compose split pattern

Compose files are split by service and always merged together via `-f` flags. `compose.base.yml` is always required — it wires the shared networks. The startup order matters: db → eventstoredb → rabbitmq → api → ui.

Two Docker bridge networks must exist before any container starts:
- `app-shared-network` — UI, API, and anything externally reachable
- `internal-shared-network` — RabbitMQ, KurrentDB, and services that must not be directly exposed

If a compose file references a service that isn't on the right network, it will silently lose connectivity rather than fail loudly.

Environment is loaded from `.env` in the same directory. `.env.staging` and `.env.prod` exist for non-local environments — they are not auto-loaded; pass `--env-file` explicitly.

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

- The two Docker networks must be created once before any `docker-compose up`. Missing networks cause silent connectivity failures, not startup errors.
- `ENV_MODE` in the compose env is `Dev` (not `Debug` or `Release`) — this controls dacpac build output path in Inventory-DB.
- Platform charts are prerequisites for workloads. If cert-manager or nginx-gateway-fabric is missing, TLS and routing will fail at the cluster level, not per-service.
- Infisical `universal-auth-credentials` chart holds the machine identity secret used to pull secrets at runtime — losing this breaks secret sync for all services.
