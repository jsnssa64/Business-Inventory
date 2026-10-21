param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/register.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/User/Register" -Payload $payload
