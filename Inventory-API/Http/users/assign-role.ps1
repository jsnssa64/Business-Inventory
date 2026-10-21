param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/assign-role.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/User/AssignUserRole" -Payload $payload
