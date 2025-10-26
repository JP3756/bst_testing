param(
    [string]$SolutionPath = (Resolve-Path (Join-Path $PSScriptRoot "..\..\bst_frontend.sln")),
    [string]$LogsDir = (Join-Path $PSScriptRoot 'test-logs'),
    [string]$Configuration = 'Debug'
)

if (-not (Test-Path $LogsDir)) { New-Item -ItemType Directory -Path $LogsDir | Out-Null }

$cleanup = Join-Path $PSScriptRoot 'cleanup-dev.ps1'
if (Test-Path $cleanup) {
    Write-Host "Running cleanup-dev.ps1 before build/test..."
    try { & $cleanup -FrontendPort 5002 } catch { Write-Warning "cleanup-dev.ps1 failed: $($_.Exception.Message)" }
} else {
    Write-Host "No cleanup-dev.ps1 found at $cleanup, continuing without cleanup."
}

$buildLog = Join-Path $LogsDir 'solution-build.log'
$testLog = Join-Path $LogsDir 'solution-test.log'

Write-Host "Building solution: $SolutionPath (configuration: $Configuration)"
$buildCmd = "dotnet build `"$SolutionPath`" -c $Configuration -v minimal"
Write-Host "Running: $buildCmd"
# Run build and capture output
& dotnet build $SolutionPath -c $Configuration -v minimal 2>&1 | Tee-Object -FilePath $buildLog
if ($LASTEXITCODE -ne 0) {
    $buildOut = Get-Content -Path $buildLog -Raw -ErrorAction SilentlyContinue
    if ($buildOut -and $buildOut -match "CS5001") {
        Write-Warning "Build failed with CS5001. Attempting targeted recovery: dotnet clean, build backend project, then rebuild solution."
        try { dotnet clean $SolutionPath | Out-Null } catch {}
        $backendProj = Resolve-Path (Join-Path $PSScriptRoot '..\bst_backend.csproj')
        Write-Host "Building backend project explicitly: $backendProj"
        & dotnet build $backendProj -c $Configuration -v minimal 2>&1 | Tee-Object -FilePath (Join-Path $LogsDir 'backend-build-recovery.log')
        if ($LASTEXITCODE -eq 0) {
            Write-Host "Backend project built. Rebuilding solution..."
            & dotnet build $SolutionPath -c $Configuration -v minimal 2>&1 | Tee-Object -FilePath $buildLog
        }
    }

    if ($LASTEXITCODE -ne 0) {
        Write-Error "Build failed. See $buildLog"
        exit $LASTEXITCODE
    }
}

Write-Host "Build succeeded. Running tests (no-build)..."
# Run tests without building to avoid parallel build races
& dotnet test $SolutionPath -c $Configuration --no-build 2>&1 | Tee-Object -FilePath $testLog
if ($LASTEXITCODE -ne 0) {
    Write-Error "Tests failed. See $testLog"
    exit $LASTEXITCODE
}

Write-Host "Build and tests completed successfully. Logs: $LogsDir"
exit 0
