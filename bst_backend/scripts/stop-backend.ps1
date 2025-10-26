<#
Stop backend by PID file or by scanning running dotnet processes for the backend project.
Usage: .\stop-backend.ps1 [-PidFile <path>]
#>

[CmdletBinding()]
param(
	[string]$PidFile = (Join-Path (Resolve-Path $PSScriptRoot) 'backend.pid')
)

if (Test-Path $PidFile) {
	try {
		$pidContent = Get-Content $PidFile -ErrorAction Stop | Select-Object -First 1
		$pidValue = 0
		if ($pidContent -and [int]::TryParse($pidContent, [ref]$pidValue)) {
			Write-Host "Stopping backend PID: $pidValue"
			Stop-Process -Id $pidValue -Force -ErrorAction SilentlyContinue
			Remove-Item $PidFile -ErrorAction SilentlyContinue
			return
		}
	} catch {
		$errMsg = $_.Exception.Message
		Write-Warning "Failed to read pid file ${PidFile}: $errMsg"
	}
}

Write-Host "Pid file not found or invalid. Scanning dotnet processes for the backend project name."
Get-Process -Name dotnet -ErrorAction SilentlyContinue | ForEach-Object {
	try {
		$procInfo = Get-CimInstance Win32_Process -Filter "ProcessId=$($_.Id)"
		$cmd = $procInfo.CommandLine
		if ($cmd -and $cmd -match 'bst_backend') {
			Write-Host "Stopping process $($_.Id) -> $cmd"
			Stop-Process -Id $_.Id -Force -ErrorAction SilentlyContinue
		}
	} catch { }
}