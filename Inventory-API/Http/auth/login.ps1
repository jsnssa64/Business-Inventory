param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/login.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/User/Login" -Payload $payload
Show-TokenInfo
