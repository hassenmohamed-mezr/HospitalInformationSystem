<#
.SYNOPSIS
    One-click startup: FastAPI (RAG) + ASP.NET MVC

.DESCRIPTION
    - Starts FastAPI (RAG_Chat/main.py) using conda in a separate window
    - Waits for API warm-up
    - Starts ASP.NET MVC (.NET 9) in current terminal

.REQUIREMENTS
    - conda installed and available in PATH
    - dotnet SDK installed
    - conda env must include: uvicorn, fastapi, dependencies

.USAGE
    powershell -ExecutionPolicy Bypass -File .\start-project.ps1 -CondaEnv "genai"
#>

[CmdletBinding()]
param(
    [string] $CondaEnv = "",
    [int] $ApiStartupDelaySeconds = 5,
    [string] $UvicornHost = "0.0.0.0",
    [int] $UvicornPort = 8000
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

# =========================
# Paths
# =========================
$ProjectRoot = $PSScriptRoot
$RagDir = Join-Path $ProjectRoot "RAG_Chat"
$CsprojPath = Join-Path $ProjectRoot "HospitalInformationSystem.csproj"

# =========================
# Logging
# =========================
function Info($msg) { Write-Host $msg -ForegroundColor Cyan }
function FastAPI($msg) { Write-Host $msg -ForegroundColor Green }
function MVC($msg) { Write-Host $msg -ForegroundColor Yellow }

# =========================
# Validate project structure
# =========================
if (-not (Test-Path $CsprojPath)) {
    throw "MVC project not found: $CsprojPath"
}

if (-not (Test-Path (Join-Path $RagDir "main.py"))) {
    throw "FastAPI entry not found: RAG_Chat/main.py"
}

# =========================
# Resolve Conda Env
# =========================
if ([string]::IsNullOrWhiteSpace($CondaEnv)) {
    $CondaEnv = $env:HIS_CONDA_ENV
}

if ([string]::IsNullOrWhiteSpace($CondaEnv)) {
    throw "Conda environment not provided. Use -CondaEnv or set HIS_CONDA_ENV."
}

# =========================
# Check dependencies
# =========================
if (-not (Get-Command conda -ErrorAction SilentlyContinue)) {
    throw "conda not found in PATH"
}

if (-not (Get-Command dotnet -ErrorAction SilentlyContinue)) {
    throw "dotnet SDK not found in PATH"
}

# =========================
# Header
# =========================
Info "=== Hospital System Startup ==="
Info "Root: $ProjectRoot"
Info "API : $RagDir"
Info "Env : $CondaEnv"
Write-Host ""

# =========================
# Start FastAPI (FIXED)
# =========================
FastAPI "[FastAPI] Starting RAG service..."

$uvicornArgs = @(
    "run",
    "-n", $CondaEnv,
    "--no-capture-output",
    "uvicorn",
    "main:app",
    "--host", $UvicornHost,
    "--port", "$UvicornPort"
)

Start-Process -FilePath "conda" `
    -WorkingDirectory $RagDir `
    -ArgumentList $uvicornArgs `
    -WindowStyle Normal

FastAPI "[FastAPI] Launched in separate window"
FastAPI "[FastAPI] URL: http://localhost:$UvicornPort"

# =========================
# Warm-up delay
# =========================
if ($ApiStartupDelaySeconds -gt 0) {
    Info "[Wait] API warm-up: $ApiStartupDelaySeconds seconds"
    Start-Sleep -Seconds $ApiStartupDelaySeconds
}

# Optional health check (safe improvement)
$maxRetries = 10
for ($i = 0; $i -lt $maxRetries; $i++) {
    try {
        Invoke-WebRequest "http://localhost:$UvicornPort/docs" -UseBasicParsing | Out-Null
        FastAPI "[FastAPI] Ready"
        break
    } catch {
        Start-Sleep 1
    }
}

# =========================
# Start MVC
# =========================
MVC "[MVC] Starting ASP.NET Core..."

Set-Location $ProjectRoot
dotnet run --project $CsprojPath