# Start backend with file logging enabled
Set-Location $PSScriptRoot
Write-Host "Starting BST Backend with logging..."
Write-Host "Log file will be created at: backend.log"
dotnet run
