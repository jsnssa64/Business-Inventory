# CLAUDE.md — Http/

This directory contains PowerShell Http for manually testing the Inventory API. No test framework — the goal is speed and low friction.

## Purpose

Quick, one-command API calls during development. Each endpoint has its own `.ps1` file. Http output raw responses only — no assertions, no frameworks.

## Structure

```
Http/
├── config.ps1              # Shared config and helper functions
├── list-endpoints.ps1      # Fetches Swagger JSON and prints grouped endpoint list
├── help.ps1                # Print commands by category (auth, users, products, inventory)
├── auth/                   # Login, logout, register
├── users/                  # User CRUD + role management
├── products/               # Product CRUD
├── inventory/              # Inventory reads and updates
└── payloads/               # JSON request bodies (one file per endpoint)
```

## How config.ps1 works

- `$BASE_URL` — target API. Currently `https://localhost:44332`
- `$COOKIE_FILE` — resolves to `Http/.cookies.json` using `$PSScriptRoot` (works correctly in dot-sourced files in PS 3+)
- Three helper functions all Http use:
  - `Invoke-ApiGet` — GET request, pretty-prints JSON response
  - `Invoke-ApiPostJson` — POST with a payload file, pretty-prints JSON response
  - `Invoke-ApiAction` — any method, shows response body only on non-empty (used for endpoints that return no body)
- `Get-ApiSession` / `Save-ApiSession` — loads and persists cookies from `.cookies.json` between script runs
- Cookie domain is derived dynamically from `$BASE_URL` so it works regardless of host

## How individual Http work

Each script is 2–4 lines:

```powershell
param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/../payloads/auth/login.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/User/Login" -Payload $payload
```

- Dot-sources `config.ps1` — all helpers and `$BASE_URL` come from there
- Payload Http default to the matching file in `payloads/` but accept `-PayloadPath` override
- Mandatory params (e.g. `-Username`, `-ProductId`) use `[Parameter(Mandatory)]` so PowerShell prompts if omitted

## Adding a new script

1. Create `Http/<resource>/<action>.ps1`
2. Dot-source config: `. "$PSScriptRoot/../config.ps1"`
3. Call the appropriate helper:
   - Read endpoint returning JSON → `Invoke-ApiGet`
   - Write endpoint returning a value (e.g. new GUID) → `Invoke-ApiPostJson`
   - Write endpoint returning no body → `Invoke-ApiAction`
4. Add the default payload under `Http/&lt;resource&gt;/payloads/<resource>/` if the endpoint takes a body
5. Add an entry to the relevant category in `help.ps1`

## Conventions

- Http never contain hardcoded URLs — always use `$BASE_URL` from config
- No assertions, no expected values — output only
- Log lines (`Write-Log`) go to the PowerShell host stream (not stdout) so they don't pollute pipeline output
- `.cookies.json` is gitignored — it holds live auth tokens
