# CI/CD rules for this project

Business Inventory is a monorepo with GitHub Actions workflows in `.github/workflows/`. This is a personal learning project: the pipelines should build production-pipeline experience, even while some subprojects are incomplete.

## Confirmed rules
<!-- appended here as they're established, one per entry -->
<!-- format: - RULE. Confirmed YYYY-MM-DD. -->
- Workflows are split by project, not by concern: one file per project (e.g. `ui.yml`, `api.yml`), holding that project's jobs and its own `paths:` trigger. Confirmed 2026-11-04.
- Keep costs to the minimum: use free tiers only, with no paid cloud services or subscriptions unless explicitly decided. Moving to Azure or AWS is a possible future step, not a current target. Confirmed 2026-11-04.
- The GitHub repo is public, so workflow files and logs are public, and nothing sensitive may be committed or printed. Confirmed 2026-11-04.
- Project workflows run on both PRs into `master` and pushes to `master`, filtered by `paths`. Confirmed 2026-11-04.
- The UI pipeline uses Node 26. Confirmed 2026-11-04.

## Open questions
<!-- pipeline decisions that came up but weren't resolved into a rule -->
- Should repo-wide checks such as `secrets.yaml` be an exception to the by-project rule?
- CD target: where do the workloads run, and how do they get deployed?
- Image registry and tag strategy.

## Unconfirmed observations (not rules - candidates only)
<!-- facts noticed in the repo that affect pipelines, not decisions -->
- Only `Inventory-UI`, `Inventory-API` and `Inventory-DB` have Dockerfiles.
- `Inventory-UI/Dockerfile` uses `node:16-alpine`, which doesn't match the pipeline's Node 26.
- `Inventory-UI` has no `.nvmrc`.
- `NotificationService` doesn't compile, per its README, so it can't have a passing build job yet.
- `AuthenticationService` uses the contract in `Grpc.Shared`, so its workflow would need to watch both folders.
- `deploy-scripts/helm/` expects `helm` on the `PATH`, a Kubernetes context already set, and required secrets already in the target namespace.
- `Infrastructure/CD/Platform/` charts are prerequisites for workloads and change on a different cadence.
- `Infrastructure/CI/Terraform` is an unused stub.
