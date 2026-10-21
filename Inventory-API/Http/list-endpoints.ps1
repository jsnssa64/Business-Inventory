. "$PSScriptRoot/config.ps1"

Write-Log "-> GET $BASE_URL/swagger/v1/swagger.json"
$r = Invoke-WebRequest -Uri "$BASE_URL/swagger/v1/swagger.json" `
    -SkipCertificateCheck -SkipHttpErrorCheck

if ($r.StatusCode -ne 200) {
    Write-Host "  Could not reach Swagger: HTTP $($r.StatusCode)" -ForegroundColor Red
    Write-Host "  Is the API running at $BASE_URL ?" -ForegroundColor DarkGray
    exit 1
}

$swagger = $r.Content | ConvertFrom-Json

$methodColor = @{
    GET    = "Green"
    POST   = "Cyan"
    PUT    = "Yellow"
    PATCH  = "Yellow"
    DELETE = "Red"
}

# Group operations by controller tag
$groups = [ordered]@{}
foreach ($path in $swagger.paths.PSObject.Properties) {
    foreach ($verb in $path.Value.PSObject.Properties) {
        $tag = $verb.Value.tags?[0] ?? "Other"
        if (-not $groups.Contains($tag)) { $groups[$tag] = [System.Collections.Generic.List[object]]::new() }
        $groups[$tag].Add(@{ Method = $verb.Name.ToUpper(); Path = $path.Name; Summary = $verb.Value.summary })
    }
}

Write-Host ""
foreach ($group in $groups.GetEnumerator()) {
    Write-Host "  $($group.Key)" -ForegroundColor White
    Write-Host ("  " + [string][char]0x2500 * 50) -ForegroundColor DarkGray

    foreach ($op in $group.Value | Sort-Object { $_.Path }) {
        $color = $methodColor[$op.Method] ?? "White"
        Write-Host "  " -NoNewline
        Write-Host ("{0,-8}" -f $op.Method) -ForegroundColor $color -NoNewline
        Write-Host $op.Path
    }
    Write-Host ""
}
