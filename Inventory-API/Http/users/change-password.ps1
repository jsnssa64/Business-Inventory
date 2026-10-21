param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/change-password.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/User/ChangePassword" -Payload $payload
