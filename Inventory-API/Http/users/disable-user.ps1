param([Parameter(Mandatory)][string]$Username)
. "$PSScriptRoot/../config.ps1"
Invoke-ApiAction -Method GET -Url "$BASE_URL/User/Disable?username=$Username"
