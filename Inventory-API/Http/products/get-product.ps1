param([Parameter(Mandatory)][string]$ProductId)
. "$PSScriptRoot/../config.ps1"
Invoke-ApiGet -Url "$BASE_URL/Product/GetProductById?productid=$ProductId"
