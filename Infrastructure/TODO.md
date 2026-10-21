# Infrastructure TODO

Mark done with `[x]` or `[done]`.

---

## Docker Compose — Local Dev

- [x] `compose.kurrentdb.yml` (formerly `compose.eventstoredb.yml`) — KurrentDB container had no `networks:` block, silently isolated from the API. Fixed — now on `backend-network`.
- [x] `compose.db.yml` — DB container was on the wrong network. Fixed — now on its own `data-network`, reachable only by `api`.
- [x] `compose.ui.yml` — bind mounts were relative to the compose file directory, not the UI project path, silently mounting empty directories. Removed entirely instead — the Dockerfile's production stage runs `serve -s dist` on a prebuilt bundle, so nothing in the container ever reads `/app/src` or `/app/public`; the mount was inert either way. UI container is now a build-time snapshot, not a live-reload target.
- [x] `compose.ui.yml` — referenced `NODE_ENV` but the env file defines `NODE_ENVIRONMENT`, so it was unset inside the container. Fixed — now sources from `NODE_ENVIRONMENT`.
- [x] `compose.api.yml` — referenced `INFISICAL_TOKEN_SERVICE_A`, absent from `.env` and unused by the app (Infisical was the intended k8s secrets path, not local Docker). Removed the env var entirely.
- [ ] Docker secrets block in `compose.db.yml` is commented out (marked as a future task).
- [x] `.env` — `DB_PROJECT_PATH`, `UI_PROJECT_PATH`, `API_PROJECT_PATH` each only went up two directories (`./../../`), resolving to `Infrastructure/Inventory-*` instead of the repo root. All three `build.context` paths were broken. Fixed — now `./../../../`.
- [x] `compose.api.yml` — `depends_on` only listed `db`, not `kurrentdb.db`/`rabbitmq`. Fixed — all three listed (no healthchecks exist, so this only affects start order, not readiness).
- [x] `compose.ui.yml` — `container_name` was hardcoded (`react-app`) instead of using `${CONTAINER_NAME}` like the other services. Fixed.
- [x] `version: '3.8'` was obsolete and inconsistently present across compose files (Compose ignores it and warns). Removed from all.

---

## Kubernetes — Workload Charts (`CD/k8s/`)

- [ ] `Components/api/values.yaml` — app name, image repository, and all four secret references are blank or commented out. Also contains JS-style `//` comments (invalid YAML). Chart cannot be applied.
- [ ] `Components/api/templates/httproute.yaml` and `ingress.yaml` — backend service name and path are hardcoded placeholders.
- [ ] `Components/api/` — no working `deploy.sh`. The only one (`old/deploy.sh`) has a shell syntax bug and dead flag parsing.
- [ ] `Components/ui/` — image has no registry, secrets have placeholder base64 values, configmap points to `localhost:5432` (PostgreSQL), HPA targets a nonexistent deployment name.
- [ ] `Components/rabbitmq/` — requires the RabbitMQ Operator CRD which is not documented or installed via any platform chart.
- [ ] `Manifests/` — everything is generic placeholder values (`app:latest`, `app.local`, `app-service`). Not wired to any real service.

---

## Kubernetes — Platform Charts (`CD/Platform/`)

- [ ] `cert-manager/` — `chart.yaml`, `values.yaml`, and all three environment values files are empty (0 bytes). No deployable content and no `deploy.sh`.
- [ ] `nginx-gateway-fabric/` — all environment values files (`dev`, `staging`, `prod`, `common`) are blank. `deploy.sh` passes them as required `-f` flags but they add nothing.
- [ ] `secrets/infisical/` — no `deploy.sh`. Template value paths don't match the values file structure; secrets would render as empty strings.
- [ ] `secrets/universal-auth-credentials/values.yaml` — `clientId` and `clientSecret` are blank. Machine identity credentials not filled in.

---

## Terraform

- [ ] `CI/Terraform/main.tf` is an empty file. Not started.
