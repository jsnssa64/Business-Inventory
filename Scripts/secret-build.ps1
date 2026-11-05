$project = "../Inventory-API/InventoryApi/InventoryApi.csproj"

foreach ($name in "AccessToken", "RefreshToken", "ConfirmationToken", "ResetPasswordToken") {
    $rsa = [System.Security.Cryptography.RSA]::Create(2048)
    $pem = $rsa.ExportPkcs8PrivateKeyPem()
    $b64 = [Convert]::ToBase64String([Text.Encoding]::UTF8.GetBytes($pem))
    dotnet user-secrets set "Security:${name}:Key" $b64 --project $project | Out-Null
}

dotnet user-secrets list --project $project | ForEach-Object { ($_ -split ' = ')[0] }