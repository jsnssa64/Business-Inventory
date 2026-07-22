# Infrastructure TODO

Mark done with `[x]` or `[done]`.

---

## Docker Compose — Local Dev

- [ ] `compose.eventstoredb.yml` — KurrentDB container has no `networks:` block. Silently isolated from the API; event store connection will fail on startup.
- [ ] `compose.db.yml` — DB container is on `app-shared-network` instead of `internal-shared-network`.
- [ ] `compose.ui.yml` — bind mounts are relative to the compose file directory, not the UI project path. Silently mount empty directories.
- [ ] `compose.ui.yml` — references `NODE_ENV` but the env file defines `NODE_ENVIRONMENT`. Variable is unset inside the container.
- [ ] `compose.api.yml` — references `INFISICAL_TOKEN_SERVICE_A` which is absent from all `.env` files.
- [ ] `.env.prod` and `.env.staging` still contain dev values (`test123!` passwords, localhost URLs). Neither updated for non-local use.
- [ ] Docker secrets block in `compose.db.yml` is commented out (marked as a future task).

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
