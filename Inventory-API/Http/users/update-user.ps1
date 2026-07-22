param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/update-user.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/User/Update" -Payload $payload
