param([Parameter(Mandatory)][string]$ProductId)
. "$PSScriptRoot/../config.ps1"
Invoke-ApiAction -Method GET -Url "$BASE_URL/Product/RemoveProduct?productId=$ProductId"
