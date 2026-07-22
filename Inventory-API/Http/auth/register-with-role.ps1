param([string]$PayloadPath = "")
. "$PSScriptRoot/../config.ps1"
$payload = if ($PayloadPath) { $PayloadPath } else { "$PSScriptRoot/payloads/register-with-role.json" }
Invoke-ApiAction -Method POST -Url "$BASE_URL/User/RegisterUserWithRole" -Payload $payload
