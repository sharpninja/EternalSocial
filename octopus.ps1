# Sets the two sensitive variables the EternalSocial gateway deploy needs.
# Run interactively; values are prompted, never stored in this file.
#   pwsh -File .\octopus.ps1
param(
    [string]$OctopusUrl = $env:OCTOPUS_URL,
    [string]$ApiKey = $env:OCTOPUS_API_KEY,
    [string]$ProjectId = 'Projects-6'
)
$ErrorActionPreference = 'Stop'
if (-not $OctopusUrl) { $OctopusUrl = Read-Host 'Octopus URL (e.g. http://payton-desktop:8065)' }
if (-not $ApiKey) { $ApiKey = Read-Host 'Octopus API key' }
$OctopusUrl = $OctopusUrl.TrimEnd('/')
$headers = @{ 'X-Octopus-ApiKey' = $ApiKey }

$googleSecret = Read-Host 'Authentication__Google__ClientSecret' -AsSecureString
$ngrokToken = Read-Host 'NGROK_AUTHTOKEN' -AsSecureString
function Plain([securestring]$s) {
    [Runtime.InteropServices.Marshal]::PtrToStringUni([Runtime.InteropServices.Marshal]::SecureStringToGlobalAllocUnicode($s))
}

$vs = Invoke-RestMethod "$OctopusUrl/api/Spaces-1/variables/variableset-$ProjectId" -Headers $headers
foreach ($pair in @(
    @{ Name = 'Authentication__Google__ClientSecret'; Value = Plain $googleSecret },
    @{ Name = 'NGROK_AUTHTOKEN'; Value = Plain $ngrokToken }
)) {
    if (-not $pair.Value) { Write-Host "skipped $($pair.Name) (empty)"; continue }
    $existing = @($vs.Variables | Where-Object Name -eq $pair.Name)
    if ($existing) { $existing | ForEach-Object { $_.Value = $pair.Value } }
    else { $vs.Variables += @{ Name = $pair.Name; Value = $pair.Value; Type = 'Sensitive'; IsSensitive = $true; Scope = @{} } }
    Write-Host "set $($pair.Name)"
}
$null = Invoke-RestMethod "$OctopusUrl/api/Spaces-1/variables/variableset-$ProjectId" -Headers $headers -Method Put -Body ($vs | ConvertTo-Json -Depth 8) -ContentType 'application/json'
Write-Host 'EternalSocial gateway secrets saved. The next deploy will use them.'
