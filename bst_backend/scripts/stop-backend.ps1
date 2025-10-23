# Stop backend by process name if running. Usage: .\stop-backend.ps1
$procs = Get-Process -Name bst_backend -ErrorAction SilentlyContinue
if ($procs) { $procs | ForEach-Object { Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue; Write-Host "Stopped pid $($_.Id)" } } else { Write-Host "No bst_backend process found." }