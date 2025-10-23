# Smoke test for BST backend API
# Usage: run from PowerShell: .\tools\smoke_test.ps1

$base = 'http://localhost:5000'

function Post-Insert($v) {
    Write-Host "Inserting $v..."
    $resp = Invoke-WebRequest -Uri "$base/api/bst/insert?value=$v" -Method Post -ErrorAction Stop
    Write-Host "  Status: $($resp.StatusCode) $($resp.StatusDescription)"
}

try {
    Post-Insert 10
    Post-Insert 5
    Post-Insert 15
    Post-Insert 3
    Post-Insert 7

    Write-Host "\nGET /api/bst/tree"
    $tree = Invoke-RestMethod -Uri "$base/api/bst/tree" -Method Get -ErrorAction Stop
    $tree | ConvertTo-Json -Depth 10 | Write-Host

    $endpoints = @('inorder','preorder','postorder','levelorder')
    foreach ($ep in $endpoints) {
        $res = Invoke-RestMethod -Uri "$base/api/bst/$ep" -Method Get -ErrorAction Stop
        Write-Host "$ep : $res"
    }

    $metrics = @('min','max','totalnodes','leafnodes','height')
    foreach ($m in $metrics) {
        $res = Invoke-RestMethod -Uri "$base/api/bst/$m" -Method Get -ErrorAction Stop
        Write-Host "$m : $res"
    }
}
catch {
    Write-Error "Smoke test failed: $_"
    exit 1
}

Write-Host "Smoke test completed successfully."