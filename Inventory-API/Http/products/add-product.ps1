param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/add-product.json" }
Invoke-ApiPostJson -Url "$BASE_URL/Product/AddProduct" -Payload $payload
