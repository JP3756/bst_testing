# Simple smoke test script for bst backend
# Usage: powershell -ExecutionPolicy Bypass -File ./smoke-test.ps1

$base = 'http://localhost:5000/api/bst'
Write-Host "Calling $base/reset"
Invoke-RestMethod -Method Post -Uri "$base/reset"

Write-Host "Inserting 10"
Invoke-RestMethod -Method Post -Uri "$base/insert?value=10" -Body 10 -ContentType 'text/plain'
Write-Host "Inserting 20"
Invoke-RestMethod -Method Post -Uri "$base/insert?value=20" -Body 20 -ContentType 'text/plain'
Write-Host "Inserting 5"
Invoke-RestMethod -Method Post -Uri "$base/insert?value=5" -Body 5 -ContentType 'text/plain'

Write-Host "Getting inorder"
$tree = Invoke-RestMethod -Method Get -Uri "$base/inorder"
Write-Host "Inorder after inserts: $tree"

Write-Host "Resetting"
Invoke-RestMethod -Method Post -Uri "$base/reset"
$after = Invoke-RestMethod -Method Get -Uri "$base/inorder"
Write-Host "Inorder after reset: $after"
