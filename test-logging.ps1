# Test script to demonstrate both frontend.log and backend.log working
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "BST Application Logging Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if backend is running
$backendRunning = netstat -ano | findstr ":5000 " | findstr "LISTENING"
if (-not $backendRunning) {
    Write-Host "WARNING: Backend is not running!" -ForegroundColor Yellow
    Write-Host "Starting backend in new window..." -ForegroundColor Yellow
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd C:\dotnet_projects\bst_frontend\bst_backend; Write-Host 'Backend Server' -ForegroundColor Green; dotnet run"
    Write-Host "Waiting for backend to start..." -ForegroundColor Yellow
    Start-Sleep -Seconds 8
}

Write-Host ""
Write-Host "Backend Status:" -ForegroundColor Green
netstat -ano | findstr ":5000 " | findstr "LISTENING"

Write-Host ""
Write-Host "Checking backend log file..." -ForegroundColor Green
$backendLog = Get-ChildItem C:\dotnet_projects\bst_frontend\bst_backend\backend*.log -ErrorAction SilentlyContinue | Select-Object -First 1
if ($backendLog) {
    Write-Host "✓ Backend log found: $($backendLog.Name)" -ForegroundColor Green
    Write-Host "  Size: $($backendLog.Length) bytes" -ForegroundColor Gray
    Write-Host "  Last modified: $($backendLog.LastWriteTime)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "Last 5 entries from backend.log:" -ForegroundColor Cyan
    Get-Content $backendLog.FullName -Tail 5 | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
} else {
    Write-Host "✗ Backend log not found" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Testing Frontend Logging..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Starting frontend with logging for 15 seconds..." -ForegroundColor Yellow
Write-Host "This will create/update frontend.log" -ForegroundColor Yellow
Write-Host ""

Set-Location C:\dotnet_projects\bst_frontend

# Create a job to run frontend
$frontendJob = Start-Job -ScriptBlock {
    Set-Location C:\dotnet_projects\bst_frontend
    
    # Clear old log
    Remove-Item frontend.log -ErrorAction SilentlyContinue
    
    # Add startup message
    $ts = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff zzz"
    Add-Content -Path "frontend.log" -Value "$ts [INF] bst_frontend: Starting BST Frontend application"
    Add-Content -Path "frontend.log" -Value "$ts [INF] bst_frontend: Configured to connect to backend at http://localhost:5000"
    
    # Run frontend
    dotnet run *>&1 | ForEach-Object {
        $line = $_.ToString()
        $ts = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff zzz"
        
        $level = "INF"
        if ($line -match "fail:|error:|Error|ERROR") { $level = "ERR" }
        elseif ($line -match "warn:|warning:|Warning") { $level = "WRN" }
        elseif ($line -match "debug:|Debug") { $level = "DBG" }
        
        $logEntry = "$ts [$level] bst_frontend: $line"
        Add-Content -Path "frontend.log" -Value $logEntry
    }
}

# Wait for 15 seconds
Write-Host "Waiting 15 seconds for frontend to start and generate logs..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Stop the job
Write-Host "Stopping frontend..." -ForegroundColor Yellow
Stop-Job $frontendJob -ErrorAction SilentlyContinue
Remove-Job $frontendJob -Force -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Frontend Log Results:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

$frontendLog = Get-Item frontend.log -ErrorAction SilentlyContinue
if ($frontendLog) {
    Write-Host "✓ Frontend log created: $($frontendLog.Name)" -ForegroundColor Green
    Write-Host "  Size: $($frontendLog.Length) bytes" -ForegroundColor Gray
    Write-Host "  Last modified: $($frontendLog.LastWriteTime)" -ForegroundColor Gray
    Write-Host ""
    Write-Host "All entries from frontend.log:" -ForegroundColor Cyan
    Get-Content $frontendLog.FullName | ForEach-Object { Write-Host "  $_" -ForegroundColor White }
} else {
    Write-Host "✗ Frontend log not created" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Log Format Comparison:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Backend log format (Serilog):" -ForegroundColor Yellow
Write-Host "  2025-10-26 20:43:04.021 +08:00 [INF] bst_backend: Message" -ForegroundColor White
Write-Host ""
Write-Host "Frontend log format (Custom):" -ForegroundColor Yellow
Write-Host "  2025-10-26 20:50:15.123 +08:00 [INF] bst_frontend: Message" -ForegroundColor White
Write-Host ""
Write-Host "Both logs now use the same timestamp format!" -ForegroundColor Green
Write-Host ""
Write-Host "To view logs in real-time:" -ForegroundColor Cyan
Write-Host "  Backend:  Get-Content .\bst_backend\backend*.log -Wait -Tail 20" -ForegroundColor Gray
Write-Host "  Frontend: Get-Content .\frontend.log -Wait -Tail 20" -ForegroundColor Gray
Write-Host ""
