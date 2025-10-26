# Start frontend and capture logs to frontend.log
Set-Location $PSScriptRoot
Write-Host "Starting BST Frontend..."
Write-Host "Logs will be written to: frontend.log"
Write-Host "Frontend will be available at: http://localhost:5002"
Write-Host ""

# Clear old log file
Remove-Item frontend.log -ErrorAction SilentlyContinue

# Add timestamp to log
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff zzz"
Add-Content -Path "frontend.log" -Value "$timestamp [INF] bst_frontend: Starting BST Frontend application"
Add-Content -Path "frontend.log" -Value "$timestamp [INF] bst_frontend: Configured to connect to backend at http://localhost:5000"

# Run frontend and redirect output to log file with timestamps
dotnet run *>&1 | ForEach-Object {
    $line = $_.ToString()
    $ts = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff zzz"
    
    # Determine log level from the message
    $level = "INF"
    if ($line -match "fail:|error:|Error|ERROR") { $level = "ERR" }
    elseif ($line -match "warn:|warning:|Warning") { $level = "WRN" }
    elseif ($line -match "debug:|Debug") { $level = "DBG" }
    
    # Format similar to Serilog
    $logEntry = "$ts [$level] bst_frontend: $line"
    
    # Write to both console and file
    Write-Host $line
    Add-Content -Path "frontend.log" -Value $logEntry
}

# Add shutdown message
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff zzz"
Add-Content -Path "frontend.log" -Value "$timestamp [INF] bst_frontend: Application stopped"
