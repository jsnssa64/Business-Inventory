param([Parameter(Mandatory)][string]$Username)
. "$PSScriptRoot/../config.ps1"
Invoke-ApiGet -Url "$BASE_URL/User/GetUserDetailsByUser?username=$Username"
