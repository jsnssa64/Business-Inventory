# API Test Http

PowerShell Http for manually testing the Inventory API. No test framework — raw responses only.

## Requirements

- **PowerShell 7+** (`pwsh`) — not Windows PowerShell 5

## First-time setup

If Http are blocked by execution policy, run once:

```powershell
Set-ExecutionPolicy -Scope CurrentUser -ExecutionPolicy RemoteSigned
```

## Config

Edit [config.ps1](config.ps1) to change the target URL:

```powershell
$BASE_URL = "https://localhost:44332"
```

Cookies are saved automatically to `Http/.cookies.json` on login and reused on every subsequent call. The file is gitignored.

---

## List all endpoints

Fetches the Swagger JSON and prints every endpoint with its HTTP method:

```powershell
.\Http\list-endpoints.ps1
```

Example output:
```
  Endpoints  (18 paths)
  ────────────────────────────────────────────────────────────
  GET     /Inventory/GetInventory
  GET     /Inventory/GetInventoryItemByProductId
  POST    /Inventory/UpdateItemInInventory
  POST    /Product/AddProduct
  GET     /Product/GetProductById
  ...
```

Requires the API to be running. Methods are colour-coded: GET=green, POST=cyan, DELETE=red.

---

## List all commands

Prints every available script grouped by category (auth, users, products, inventory):

```powershell
.\Http\help.ps1
```

---

## Usage

Run all Http from the **repo root** in a PowerShell terminal.

### 1. Authenticate

Edit credentials first: `Http/auth/payloads/login.json`

```powershell
.\Http\auth\login.ps1
#   -> POST https://localhost:44332/User/Login
#   <- HTTP 200
```

All subsequent Http reuse the saved cookies automatically.

---

### 2. Users

```powershell
# Your own profile
.\Http\users\get-user.ps1
.\Http\users\get-user-details.ps1

# Admin only
.\Http\users\get-users.ps1
.\Http\users\get-user-details-by-user.ps1 -Username johndoe
.\Http\users\enable-user.ps1  -Username johndoe
.\Http\users\disable-user.ps1 -Username johndoe
.\Http\users\assign-role.ps1  # edit payloads\users\assign-role.json first

# Edit payloads\users\update-user.json, then:
.\Http\users\update-user.ps1

# Edit payloads\users\change-password.json, then:
.\Http\users\change-password.ps1
```

---

### 3. Products

```powershell
# List all products
.\Http\products\get-products.ps1

# Get one product by GUID
.\Http\products\get-product.ps1 -ProductId "3fa85f64-5717-4562-b3fc-2c963f66afa6"

# Add a product (returns the new GUID) — edit payloads\products\add-product.json first
.\Http\products\add-product.ps1

# Update — set productId in payloads\products\update-product.json first
.\Http\products\update-product.ps1

# Remove by GUID
.\Http\products\remove-product.ps1 -ProductId "3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

---

### 4. Inventory

```powershell
# Full inventory for current user
.\Http\inventory\get-inventory.ps1

# Inventory for a specific product
.\Http\inventory\get-inventory-item.ps1 -ProductId "3fa85f64-5717-4562-b3fc-2c963f66afa6"

# Update quantity — set productId in payloads\inventory\update-inventory.json first
.\Http\inventory\update-inventory.ps1
```

---

### 5. Auth (other)

```powershell
# Register (no auth required) — edit payloads\auth\register.json first
.\Http\auth\register.ps1

# Register with role (Admin only) — edit payloads\auth\register-with-role.json first
.\Http\auth\register-with-role.ps1

# Forgotten password email
.\Http\auth\forgotten-password.ps1 -Username johndoe

# Logout
.\Http\auth\logout.ps1
```

---

## Custom payloads

Every script that posts a body accepts an optional `-PayloadPath` override:

```powershell
.\Http\auth\login.ps1 -PayloadPath "C:\my-payloads\admin.json"
.\Http\products\add-product.ps1 -PayloadPath "C:\my-payloads\widget-b.json"
```

---

## Output conventions

| Response | Output |
|---|---|
| JSON body | Pretty-printed via `ConvertTo-Json` |
| Empty body (action endpoints) | `<- HTTP 200` |
| Error body | Pretty-printed automatically |

Log lines (grey) go to the host stream and do not pollute pipeline output.

---

## File structure

```
Http/
├── config.ps1
├── list-endpoints.ps1
├── help.ps1
├── README.md
├── auth/
│   ├── login.ps1
│   ├── logout.ps1
│   ├── register.ps1
│   ├── register-with-role.ps1
│   ├── forgotten-password.ps1
│   └── payloads/
│       ├── login.json
│       ├── register.json
│       └── register-with-role.json
├── users/
│   ├── get-users.ps1
│   ├── get-user.ps1
│   ├── get-user-details.ps1
│   ├── get-user-details-by-user.ps1
│   ├── update-user.ps1
│   ├── change-password.ps1
│   ├── assign-role.ps1
│   ├── enable-user.ps1
│   ├── disable-user.ps1
│   └── payloads/
│       ├── update-user.json
│       ├── change-password.json
│       └── assign-role.json
├── products/
│   ├── get-products.ps1
│   ├── get-product.ps1
│   ├── add-product.ps1
│   ├── update-product.ps1
│   ├── remove-product.ps1
│   └── payloads/
│       ├── add-product.json
│       └── update-product.json
└── inventory/
    ├── get-inventory.ps1
    ├── get-inventory-item.ps1
    ├── update-inventory.ps1
    └── payloads/
        └── update-inventory.json
```
