param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/update-product.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/Product/UpdateProduct" -Payload $payload
