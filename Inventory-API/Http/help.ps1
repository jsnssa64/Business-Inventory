param([string]$Category = "")

$commands = @{
    auth = @(
        @{ Cmd = ".\Http\auth\login.ps1";                              Note = "Login — sets session cookies" }
        @{ Cmd = ".\Http\auth\logout.ps1";                             Note = "Logout" }
        @{ Cmd = ".\Http\auth\register.ps1";                           Note = "Register new user  |  payload: auth\payloads\register.json" }
        @{ Cmd = ".\Http\auth\register-with-role.ps1";                 Note = "Register with role (Admin)  |  payload: auth\payloads\register-with-role.json" }
        @{ Cmd = ".\Http\auth\forgotten-password.ps1 -Username <name>"; Note = "Send forgotten password email" }
    )
    users = @(
        @{ Cmd = ".\Http\users\get-user.ps1";                                    Note = "Get own profile" }
        @{ Cmd = ".\Http\users\get-user-details.ps1";                            Note = "Get own full details" }
        @{ Cmd = ".\Http\users\get-users.ps1";                                   Note = "Get all users (Admin)" }
        @{ Cmd = ".\Http\users\get-user-details-by-user.ps1 -Username <name>";   Note = "Get user details by username (Admin)" }
        @{ Cmd = ".\Http\users\update-user.ps1";                                 Note = "Update own profile  |  payload: users\payloads\update-user.json" }
        @{ Cmd = ".\Http\users\change-password.ps1";                             Note = "Change password  |  payload: users\payloads\change-password.json" }
        @{ Cmd = ".\Http\users\assign-role.ps1";                                 Note = "Assign role (Admin)  |  payload: users\payloads\assign-role.json" }
        @{ Cmd = ".\Http\users\enable-user.ps1  -Username <name>";               Note = "Enable user (Admin)" }
        @{ Cmd = ".\Http\users\disable-user.ps1 -Username <name>";               Note = "Disable user (Admin)" }
    )
    products = @(
        @{ Cmd = ".\Http\products\get-products.ps1";                        Note = "List all products" }
        @{ Cmd = ".\Http\products\get-product.ps1 -ProductId <guid>";       Note = "Get product by ID" }
        @{ Cmd = ".\Http\products\add-product.ps1";                         Note = "Add product (returns GUID)  |  payload: products\payloads\add-product.json" }
        @{ Cmd = ".\Http\products\update-product.ps1";                      Note = "Update product  |  payload: products\payloads\update-product.json" }
        @{ Cmd = ".\Http\products\remove-product.ps1 -ProductId <guid>";    Note = "Remove product" }
    )
    inventory = @(
        @{ Cmd = ".\Http\inventory\get-inventory.ps1";                          Note = "Get full inventory" }
        @{ Cmd = ".\Http\inventory\get-inventory-item.ps1 -ProductId <guid>";   Note = "Get inventory for a product" }
        @{ Cmd = ".\Http\inventory\update-inventory.ps1";                       Note = "Update quantity  |  payload: inventory\payloads\update-inventory.json" }
    )
}

function Show-Category([string]$name) {
    Write-Host ""
    Write-Host "  $($name.ToUpper())" -ForegroundColor Cyan
    Write-Host ("  " + [string][char]0x2500 * 60) -ForegroundColor DarkGray
    foreach ($entry in $commands[$name]) {
        Write-Host "  " -NoNewline
        Write-Host $entry.Cmd -ForegroundColor White -NoNewline
        Write-Host "  " -NoNewline
        Write-Host $entry.Note -ForegroundColor DarkGray
    }
}

$valid = $commands.Keys | Sort-Object

if ($Category -eq "") {
    foreach ($name in $valid) { Show-Category $name }
    Write-Host ""
    Write-Host "  Tip: run with a category to filter  " -NoNewline -ForegroundColor DarkGray
    Write-Host ".\Http\help.ps1 -Category auth" -ForegroundColor Yellow
    Write-Host ""
} elseif ($commands.ContainsKey($Category.ToLower())) {
    Show-Category $Category.ToLower()
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "  Unknown category '$Category'. Available: $($valid -join ', ')" -ForegroundColor Red
    Write-Host ""
}
