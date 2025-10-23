param(
    [string]$BackendPath = 'C:\dotnet_projects\bst_backend',
    [string]$FrontendPath = 'C:\dotnet_projects\bst_frontend',
    [int]$BackendPort = 5000,
    [int]$FrontendPort = 5001
)

function Wait-ForPort($port, $timeoutSec=30) {
    $end = (Get-Date).AddSeconds($timeoutSec)
    while((Get-Date) -lt $end) {
        if (netstat -ano | Select-String ":[${port}]" ) { return $true }
        Start-Sleep -Milliseconds 500
    }
    return $false
}

Write-Host "Starting backend..."
$bProc = Start-Process -FilePath dotnet -ArgumentList "run --project $BackendPath\bst_backend.csproj" -WorkingDirectory $BackendPath -PassThru
Start-Sleep -Seconds 1
if (-not (Wait-ForPort -port $BackendPort -timeoutSec 30)) {
    Write-Error "Backend did not start listening on port $BackendPort within timeout."
    exit 1
}
Write-Host "Backend started (PID=$($bProc.Id))."

Write-Host "Starting frontend..."
$fProc = Start-Process -FilePath dotnet -ArgumentList "run --project $FrontendPath\bst_frontend.csproj" -WorkingDirectory $FrontendPath -PassThru
Start-Sleep -Seconds 1
if (-not (Wait-ForPort -port $FrontendPort -timeoutSec 30)) {
    Write-Error "Frontend did not start listening on port $FrontendPort within timeout."
    exit 1
}
Write-Host "Frontend started (PID=$($fProc.Id))."

Write-Host "Running smoke test (insert 10,20,5) ..."
try {
    Invoke-WebRequest -Uri "http://localhost:$BackendPort/api/bst/insert?value=10" -Method Post -UseBasicParsing -ErrorAction Stop | Out-Null
    Invoke-WebRequest -Uri "http://localhost:$BackendPort/api/bst/insert?value=20" -Method Post -UseBasicParsing -ErrorAction Stop | Out-Null
    Invoke-WebRequest -Uri "http://localhost:$BackendPort/api/bst/insert?value=5" -Method Post -UseBasicParsing -ErrorAction Stop | Out-Null
    $tree = Invoke-WebRequest -Uri "http://localhost:$BackendPort/api/bst/tree" -UseBasicParsing -ErrorAction Stop | Select-Object -Expand Content
    Write-Host "Smoke test tree:"
    Write-Host $tree
    Write-Host "Smoke test completed successfully. Backend PID=$($bProc.Id), Frontend PID=$($fProc.Id)"
} catch {
    Write-Error "Smoke test failed: $_"
    exit 1
}

Write-Host "All done. Keep this PowerShell window open to keep processes alive."
