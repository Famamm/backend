<#
.SYNOPSIS
    Manual mirror of .github/workflows/docker-build.yml

.DESCRIPTION
    Builds (and optionally pushes) the FleetOps API Docker image exactly the way
    the GitHub Actions workflow does, but from your local machine. All values are
    read from a .env file (default: ./.env) so the same file that docker compose
    uses also drives this script.

    Workflow parity:
      - IMAGE_NAME  = DOCKERHUB_IMAGE            (fallback: fleetopsapp/fleetops-api)
      - IMAGE_TAG   = 'production' on main, else 'dev'
      - PR mode     : push = false, tag = <image>:pr-<number>, build-arg Jwt__Key
      - Push mode   : push = true,  tags = <image>:<IMAGE_TAG> and <image>:dev
                      build-args = API_PORT, API_ENVIRONMENT, Jwt__Key

.PARAMETER Branch
    Branch being simulated. 'main' -> production tag, anything else -> dev tag.
    Default: current git branch.

.PARAMETER PrNumber
    If set, runs in pull-request mode: builds only (no push) and tags pr-<number>.

.PARAMETER EnvFile
    Path to the .env file. Default: .env in the script directory.

.PARAMETER NoCache
    Pass --no-cache to the docker build.

.PARAMETER SkipLogin
    Skip the docker login step (useful if you already ran `docker login`).

.EXAMPLE
    .\build-push.ps1
    Builds and pushes using the current git branch.

.EXAMPLE
    .\build-push.ps1 -Branch main
    Builds and pushes the production tag.

.EXAMPLE
    .\build-push.ps1 -PrNumber 42
    Builds only (no push), tagged pr-42.
#>

[CmdletBinding()]
param(
    [string]$Branch,
    [int]$PrNumber,
    [string]$EnvFile = (Join-Path $PSScriptRoot '.env'),
    [switch]$NoCache,
    [switch]$SkipLogin
)

$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest

# ---------------------------------------------------------------------------
# Helpers
# ---------------------------------------------------------------------------
function Write-Step {
    param([string]$Message)
    Write-Host ""
    Write-Host "==> $Message" -ForegroundColor Cyan
}

function Get-EnvValue {
    param(
        [Parameter(Mandatory)][hashtable]$Map,
        [Parameter(Mandatory)][string]$Key,
        [string]$Default
    )
    if ($Map.ContainsKey($Key) -and -not [string]::IsNullOrWhiteSpace($Map[$Key])) {
        return $Map[$Key]
    }
    return $Default
}

function Read-DotEnv {
    param([Parameter(Mandatory)][string]$Path)

    $map = @{}
    if (-not (Test-Path -LiteralPath $Path)) {
        throw "Env file not found: $Path. Copy .env.example to .env and fill in the values."
    }

    foreach ($raw in Get-Content -LiteralPath $Path) {
        $line = $raw.Trim()
        if ($line.Length -eq 0 -or $line.StartsWith('#')) { continue }

        $idx = $line.IndexOf('=')
        if ($idx -lt 1) { continue }

        $key = $line.Substring(0, $idx).Trim()
        $value = $line.Substring($idx + 1).Trim()

        # Strip surrounding quotes if present.
        if ($value.Length -ge 2 -and
            (($value.StartsWith('"') -and $value.EndsWith('"')) -or
             ($value.StartsWith("'") -and $value.EndsWith("'")))) {
            $value = $value.Substring(1, $value.Length - 2)
        }

        $map[$key] = $value
    }

    return $map
}

# ---------------------------------------------------------------------------
# Load configuration
# ---------------------------------------------------------------------------
$envMap = Read-DotEnv -Path $EnvFile
Write-Host "Loaded env from $EnvFile" -ForegroundColor DarkGray

# Resolve branch (mirrors github.ref_name)
if ([string]::IsNullOrWhiteSpace($Branch)) {
    $Branch = (git rev-parse --abbrev-ref HEAD 2>$null)
    if ([string]::IsNullOrWhiteSpace($Branch) -or $Branch -eq 'HEAD') {
        throw "Could not detect the current git branch. Pass -Branch explicitly."
    }
}

# IMAGE_NAME: vars.DOCKERHUB_IMAGE || 'fleetopsapp/fleetops-api'
$imageName = Get-EnvValue -Map $envMap -Key 'DOCKERHUB_IMAGE' -Default 'fleetopsapp/fleetops-api'

# IMAGE_TAG: main -> production, else dev
$imageTag = if ($Branch -eq 'main') { 'production' } else { 'dev' }

# API_PORT: vars.API_PORT || '5144'
$apiPort = Get-EnvValue -Map $envMap -Key 'API_PORT' -Default '5144'

# API_ENVIRONMENT: main -> Production, else vars.API_ENVIRONMENT || 'Development'
$apiEnvironment = if ($Branch -eq 'main') {
    'Production'
} else {
    Get-EnvValue -Map $envMap -Key 'API_ENVIRONMENT' -Default 'Development'
}

$jwtKey = Get-EnvValue -Map $envMap -Key 'Jwt__Key' -Default ''

$isPr = $PSBoundParameters.ContainsKey('PrNumber')

Write-Host ""
Write-Host "  Branch          : $Branch"
Write-Host "  Mode            : $(if ($isPr) { "pull-request (build only)" } else { 'push' })"
Write-Host "  Image name      : $imageName"
Write-Host "  Image tag       : $(if ($isPr) { "pr-$PrNumber" } else { $imageTag })"
Write-Host "  API_PORT        : $apiPort"
Write-Host "  API_ENVIRONMENT : $apiEnvironment"

# ---------------------------------------------------------------------------
# Login (mirrors docker/login-action)
# ---------------------------------------------------------------------------
if (-not $SkipLogin) {
    Write-Step "Logging in to Docker Hub"

    $dockerUser = Get-EnvValue -Map $envMap -Key 'DOCKERHUB_USERNAME' -Default ''
    $dockerToken = Get-EnvValue -Map $envMap -Key 'DOCKERHUB_TOKEN' -Default ''

    if ([string]::IsNullOrWhiteSpace($dockerUser) -or $dockerUser -eq 'change-me') {
        $dockerUser = Read-Host "Docker Hub username"
    }
    if ([string]::IsNullOrWhiteSpace($dockerToken) -or $dockerToken -eq 'change-me') {
        $secure = Read-Host "Docker Hub token" -AsSecureString
        $dockerToken = [System.Net.NetworkCredential]::new('', $secure).Password
    }

    $dockerToken | docker login --username $dockerUser --password-stdin
    if ($LASTEXITCODE -ne 0) { throw "docker login failed." }
}

# ---------------------------------------------------------------------------
# Build (mirrors docker/build-push-action)
# ---------------------------------------------------------------------------
$tags = if ($isPr) {
    @("${imageName}:pr-$PrNumber")
} else {
    # Workflow lists <IMAGE_TAG> and :dev; on develop these are the same, so dedupe.
    @("${imageName}:$imageTag", "${imageName}:dev") | Select-Object -Unique
}

$buildArgs = if ($isPr) {
    @("Jwt__Key=$jwtKey")
} else {
    @(
        "API_PORT=$apiPort",
        "API_ENVIRONMENT=$apiEnvironment",
        "Jwt__Key=$jwtKey"
    )
}

$dockerArgs = @('build', '--file', './Dockerfile')
foreach ($t in $tags) { $dockerArgs += @('--tag', $t) }
foreach ($a in $buildArgs) { $dockerArgs += @('--build-arg', $a) }
if ($NoCache) { $dockerArgs += '--no-cache' }
$dockerArgs += '.'

Write-Step "Building image"
Write-Host "docker $($dockerArgs -join ' ')" -ForegroundColor DarkGray
& docker @dockerArgs
if ($LASTEXITCODE -ne 0) { throw "docker build failed." }

# ---------------------------------------------------------------------------
# Push (mirrors push: true / push: false)
# ---------------------------------------------------------------------------
if ($isPr) {
    Write-Step "Pull-request mode: skipping push (push: false)"
} else {
    foreach ($t in $tags) {
        Write-Step "Pushing $t"
        docker push $t
        if ($LASTEXITCODE -ne 0) { throw "docker push failed for $t." }
    }
}

Write-Step "Done"
