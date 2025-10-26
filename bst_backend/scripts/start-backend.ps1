# Start the backend (detached) with optional log redirection and pid file.
<#
Usage:
	.\start-backend.ps1 [-ProjectPath <path>] [-WorkingDir <path>] [-OutLog <path>] [-ErrLog <path>] [-PidFile <path>]

Defaults:
	ProjectPath = ..\bst_backend.csproj
	WorkingDir  = parent folder of this script
	OutLog      = ./backend.log
	ErrLog      = ./backend.err.log
	PidFile     = ./backend.pid
#>

[CmdletBinding()]
param(
		[string]$ProjectPath = (Join-Path $PSScriptRoot "..\bst_backend.csproj"),
		[string]$WorkingDir = (Resolve-Path (Join-Path $PSScriptRoot "..")),
		[string]$OutLog = (Join-Path (Resolve-Path $PSScriptRoot) 'backend.log'),
		[string]$ErrLog = (Join-Path (Resolve-Path $PSScriptRoot) 'backend.err.log'),
		[string]$PidFile = (Join-Path (Resolve-Path $PSScriptRoot) 'backend.pid')
)

Write-Host "Starting backend from project: $ProjectPath"
try {
	# If there's an existing running backend exe from a previous run, stop it so the build won't fail
	$exePath = Join-Path (Split-Path -Path $ProjectPath -Parent) "bin\Debug\net8.0\bst_backend.exe"
	if (Test-Path $exePath) {
		$locking = Get-CimInstance -ClassName Win32_Process -ErrorAction SilentlyContinue | Where-Object { $_.CommandLine -and $_.CommandLine -like "*bst_backend.exe*" }
		if ($locking) {
			foreach ($proc in $locking) {
				try {
					Write-Host "Stopping existing backend process (PID: $($proc.ProcessId)) to avoid file lock..."
					Stop-Process -Id $proc.ProcessId -Force -ErrorAction SilentlyContinue
				} catch { }
			}
			Start-Sleep -Milliseconds 300
		}
	}
} catch {
	# best-effort only — don't fail the script if this check breaks
}
if (Test-Path $OutLog) { Remove-Item $OutLog -Force -ErrorAction SilentlyContinue }
if (Test-Path $ErrLog) { Remove-Item $ErrLog -Force -ErrorAction SilentlyContinue }

$argString = "run --project `"$ProjectPath`""
$proc = Start-Process -FilePath "dotnet" -ArgumentList $argString -WorkingDirectory $WorkingDir -RedirectStandardOutput $OutLog -RedirectStandardError $ErrLog -PassThru

if ($proc -and $proc.Id) {
		Write-Host "Started backend with PID: $($proc.Id)"
		try { $proc.Id | Out-File -FilePath $PidFile -Encoding ascii -Force } catch {}
		Write-Host "Logs: $OutLog and $ErrLog"
		Write-Host "To stop: .\stop-backend.ps1 or Stop-Process -Id $($proc.Id)"
} else {
		Write-Error "Failed to start backend process. See $ErrLog for details."
}