param(
    [string]$BackendPath = (Join-Path $PSScriptRoot "..\bst_backend.csproj"),
    [string]$FrontendPath = (Join-Path $PSScriptRoot "..\..\bst_frontend.csproj"),
    [int]$BackendPort = 5000,
    [int]$FrontendPort = 5002,
    [string]$LogsDir = (Join-Path $PSScriptRoot 'logs')
)

function Wait-ForPort([int]$port, [int]$timeoutSec=30) {
    $end = (Get-Date).AddSeconds($timeoutSec)
    while((Get-Date) -lt $end) {
        if (netstat -ano | Select-String ":$port") { return $true }
        Start-Sleep -Milliseconds 500
    }
    return $false
}

if (-not (Test-Path $LogsDir)) { New-Item -ItemType Directory -Path $LogsDir | Out-Null }

# Run a best-effort cleanup to stop any running backend exe and free frontend port before starting
if (Test-Path (Join-Path $PSScriptRoot 'cleanup-dev.ps1')) {
    try { & (Join-Path $PSScriptRoot 'cleanup-dev.ps1') -FrontendPort $FrontendPort } catch { }
}

$backendOut = Join-Path $LogsDir 'backend.log'
$backendErr = Join-Path $LogsDir 'backend.err.log'
$backendPid = Join-Path $LogsDir 'backend.pid'

$frontendOut = Join-Path $LogsDir 'frontend.log'
$frontendErr = Join-Path $LogsDir 'frontend.err.log'
$frontendPid = Join-Path $LogsDir 'frontend.pid'

Write-Host "Starting backend: $BackendPath (http://localhost:$BackendPort)"
& $PSScriptRoot\start-backend.ps1 -ProjectPath $BackendPath -OutLog $backendOut -ErrLog $backendErr -PidFile $backendPid

Write-Host "Waiting for backend to respond on port $BackendPort..."
if (-not (Wait-ForPort -port $BackendPort -timeoutSec 60)) {
    Write-Warning "Backend did not start within timeout. Check $backendErr and $backendOut"
}

Write-Host "Starting frontend: $FrontendPath (http://localhost:$FrontendPort)"
# Start frontend with log redirection
Start-Process -FilePath dotnet -ArgumentList "run --project `"$FrontendPath`" --urls http://localhost:$FrontendPort" -WorkingDirectory (Resolve-Path (Join-Path $PSScriptRoot '..')) -RedirectStandardOutput $frontendOut -RedirectStandardError $frontendErr -PassThru | ForEach-Object {
    try { $_.Id | Out-File -FilePath $frontendPid -Encoding ascii -Force } catch {}
    Write-Host "Started frontend PID: $($_.Id)"
}

Write-Host "Waiting for frontend to respond on port $FrontendPort..."
if (-not (Wait-ForPort -port $FrontendPort -timeoutSec 60)) {
    Write-Warning "Frontend did not start within timeout. Check $frontendErr and $frontendOut"
}

Write-Host "Running smoke tests against backend..."
try {
    $base = "http://localhost:$BackendPort/api/bst"
    Invoke-RestMethod -Method Post -Uri "$base/reset"
    Invoke-RestMethod -Method Post -Uri "$base/insert?value=10" -Body 10 -ContentType 'text/plain'
    Invoke-RestMethod -Method Post -Uri "$base/insert?value=20" -Body 20 -ContentType 'text/plain'
    Invoke-RestMethod -Method Post -Uri "$base/insert?value=5" -Body 5 -ContentType 'text/plain'
    $tree = Invoke-RestMethod -Method Get -Uri "$base/inorder"
    Write-Host "Inorder after inserts: $tree"
    Invoke-RestMethod -Method Post -Uri "$base/reset"
    $after = Invoke-RestMethod -Method Get -Uri "$base/inorder"
    Write-Host "Inorder after reset: $after"
    Write-Host "Smoke tests completed. Logs: $LogsDir"
} catch {
    Write-Warning "Smoke tests failed: $($_.Exception.Message)"
    Write-Host "Check logs: $LogsDir"
}

Write-Host "All done. Keep this PowerShell window open to keep processes alive."
