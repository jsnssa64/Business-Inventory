$BASE_URL    = "https://localhost:44332"
$COOKIE_FILE = Join-Path $PSScriptRoot ".cookies.json"

function Decode-JwtPayload([string]$token) {
    $parts = $token -split '\.'
    if ($parts.Count -lt 2) { return $null }
    $b64 = $parts[1] -replace '-', '+' -replace '_', '/'
    switch ($b64.Length % 4) {
        2 { $b64 += '==' }
        3 { $b64 += '=' }
    }
    try {
        $bytes = [Convert]::FromBase64String($b64)
        return [Text.Encoding]::UTF8.GetString($bytes) | ConvertFrom-Json
    } catch { return $null }
}

function Get-ApiSession {
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    if (Test-Path $COOKIE_FILE) {
        $data = Get-Content $COOKIE_FILE -Raw | ConvertFrom-Json
        foreach ($prop in $data.PSObject.Properties) {
            if ($prop.Name -eq '_meta') { continue }
            $c = New-Object System.Net.Cookie($prop.Name, $prop.Value, "/", ([System.Uri]$BASE_URL).Host)
            $session.Cookies.Add($c)
        }
    }
    return $session
}

function Save-ApiSession([Microsoft.PowerShell.Commands.WebRequestSession]$session, $response = $null) {
    $uri  = [System.Uri]$BASE_URL
    $data = @{}

    # Baseline: read whatever the session container already holds
    foreach ($c in $session.Cookies.GetCookies($uri)) { $data[$c.Name] = $c.Value }

    # Override with Set-Cookie headers straight from the response — these are
    # authoritative and fix cases where CookieContainer doesn't surface new
    # tokens via GetCookies() (e.g. first login, token refresh after expiry)
    if ($response) {
        foreach ($header in @($response.Headers['Set-Cookie'])) {
            if (-not $header) { continue }
            $nameValue = ($header -split ';')[0].Trim()
            $eqIndex   = $nameValue.IndexOf('=')
            if ($eqIndex -gt 0) {
                $name  = $nameValue.Substring(0, $eqIndex).Trim()
                $value = $nameValue.Substring($eqIndex + 1).Trim()
                $data[$name] = $value
            }
        }
    }

    $meta = @{ savedAt = (Get-Date).ToString('yyyy-MM-dd HH:mm:ss') }
    foreach ($tokenKey in @('authAccessToken', 'authRefreshToken')) {
        if ($data.ContainsKey($tokenKey)) {
            $claims = Decode-JwtPayload $data[$tokenKey]
            if ($claims -and $claims.iat -and $claims.exp) {
                $label = if ($tokenKey -eq 'authAccessToken') { 'access' } else { 'refresh' }
                $meta[$label] = @{
                    issuedAt  = [DateTimeOffset]::FromUnixTimeSeconds($claims.iat).LocalDateTime.ToString('yyyy-MM-dd HH:mm:ss')
                    expiresAt = [DateTimeOffset]::FromUnixTimeSeconds($claims.exp).LocalDateTime.ToString('yyyy-MM-dd HH:mm:ss')
                }
            }
        }
    }
    $data['_meta'] = $meta
    $data | ConvertTo-Json -Depth 5 | Set-Content $COOKIE_FILE
}

function Write-Log([string]$msg) {
    Write-Host "  $msg" -ForegroundColor DarkGray
}

function Show-Body([string]$content) {
    if (-not $content) { return }
    try   { $content | ConvertFrom-Json | ConvertTo-Json -Depth 20 }
    catch { Write-Output $content }
}

function Show-TokenInfo {
    if (-not (Test-Path $COOKIE_FILE)) { return }
    $data = Get-Content $COOKIE_FILE -Raw | ConvertFrom-Json
    $now  = Get-Date

    $tokenMap = [ordered]@{
        authAccessToken  = 'Access Token'
        authRefreshToken = 'Refresh Token'
    }

    foreach ($key in $tokenMap.Keys) {
        $raw = $data.PSObject.Properties[$key]?.Value
        if (-not $raw) { continue }
        $claims = Decode-JwtPayload $raw
        if (-not $claims) { continue }

        $iat       = [DateTimeOffset]::FromUnixTimeSeconds($claims.iat).LocalDateTime
        $exp       = [DateTimeOffset]::FromUnixTimeSeconds($claims.exp).LocalDateTime
        $remaining = $exp - $now
        if ($remaining.TotalSeconds -le 0) {
            $timeStr = "EXPIRED"
            $timeColor = 'Red'
        } else {
            $mins    = [Math]::Floor($remaining.TotalMinutes)
            $secs    = $remaining.Seconds
            $timeStr = "expires in ${mins}m ${secs}s"
            $timeColor = 'Cyan'
        }

        Write-Host ""
        Write-Host "  $($tokenMap[$key])" -ForegroundColor White
        if ($claims.sub)  { Write-Host ("    sub   : {0}" -f $claims.sub)  -ForegroundColor Gray }
        if ($claims.email){ Write-Host ("    email : {0}" -f $claims.email) -ForegroundColor Gray }
        if ($claims.role) { Write-Host ("    role  : {0}" -f $claims.role)  -ForegroundColor Gray }
        Write-Host ("    iat   : {0}" -f $iat.ToString('yyyy-MM-dd HH:mm:ss')) -ForegroundColor Gray
        Write-Host ("    exp   : {0}" -f $exp.ToString('yyyy-MM-dd HH:mm:ss')) -ForegroundColor Gray
        Write-Host ("    status: $timeStr") -ForegroundColor $timeColor
    }
    Write-Host ""
}

function Show-TokenStatus {
    if (-not (Test-Path $COOKIE_FILE)) { return }
    $data = Get-Content $COOKIE_FILE -Raw | ConvertFrom-Json
    if (-not $data._meta) { return }
    $now = Get-Date
    Write-Host ""
    Write-Host "  Token status (401 received):" -ForegroundColor Yellow
    foreach ($label in @('access', 'refresh')) {
        $t = $data._meta.$label
        if (-not $t) { continue }
        $exp       = [datetime]::ParseExact($t.expiresAt, 'yyyy-MM-dd HH:mm:ss', $null)
        $remaining = $exp - $now
        if ($remaining.TotalSeconds -le 0) {
            $status = "EXPIRED $([Math]::Abs([Math]::Floor($remaining.TotalMinutes)))m ago"
            $color  = 'Red'
        } else {
            $mins   = [Math]::Floor($remaining.TotalMinutes)
            $secs   = $remaining.Seconds
            $status = "expires in ${mins}m ${secs}s"
            $color  = 'Cyan'
        }
        Write-Host ("  {0,-8} issued {1}  |  {2}" -f "$label`:", $t.issuedAt, $status) -ForegroundColor $color
    }
    Write-Host ""
}

function Invoke-ApiGet {
    param([string]$Url)
    $session = Get-ApiSession
    Write-Log "-> GET $Url"
    $r = Invoke-WebRequest -Uri $Url -WebSession $session -SkipCertificateCheck -SkipHttpErrorCheck
    Save-ApiSession $session $r
    Write-Log "<- HTTP $($r.StatusCode)"
    if ($r.StatusCode -eq 401) { Show-TokenStatus }
    Show-Body $r.Content
}

function Invoke-ApiPostJson {
    param([string]$Url, [string]$Payload)
    $session = Get-ApiSession
    Write-Log "-> POST $Url"
    Write-Log "   payload: $Payload"
    $r = Invoke-WebRequest -Uri $Url -Method POST `
        -Body (Get-Content $Payload -Raw) `
        -ContentType "application/json" `
        -WebSession $session -SkipCertificateCheck -SkipHttpErrorCheck
    Save-ApiSession $session $r
    Write-Log "<- HTTP $($r.StatusCode)"
    if ($r.StatusCode -eq 401) { Show-TokenStatus }
    Show-Body $r.Content
}

function Invoke-ApiAction {
    param([string]$Method, [string]$Url, [string]$Payload = "")
    $session = Get-ApiSession
    Write-Log "-> $Method $Url"
    $params = @{
        Uri                  = $Url
        Method               = $Method
        WebSession           = $session
        SkipCertificateCheck = $true
        SkipHttpErrorCheck   = $true
    }
    if ($Payload) {
        Write-Log "   payload: $Payload"
        $params.Body        = Get-Content $Payload -Raw
        $params.ContentType = "application/json"
    }
    $r = Invoke-WebRequest @params
    Save-ApiSession $session $r
    Write-Log "<- HTTP $($r.StatusCode)"
    if ($r.StatusCode -eq 401) { Show-TokenStatus }
    Show-Body $r.Content
}
