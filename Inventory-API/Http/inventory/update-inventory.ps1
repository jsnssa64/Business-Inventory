param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/update-inventory.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/Inventory/UpdateItemInInventory" -Payload $payload
