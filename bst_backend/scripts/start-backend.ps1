# Start the backend in a separate window so the current terminal stays free for requests.
# Usage: .\start-backend.ps1

$projectPath = Join-Path $PSScriptRoot "..\bst_backend.csproj"
$workingDir = Resolve-Path (Join-Path $PSScriptRoot "..")

Write-Host "Starting backend from project: $projectPath"
$proc = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"$projectPath`"" -WorkingDirectory $workingDir -PassThru
Write-Host "Started backend with PID: $($proc.Id)" 
Write-Host "To stop: .\stop-backend.ps1 or Stop-Process -Id $($proc.Id)"