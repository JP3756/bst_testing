@echo off
REM Wrapper to run the PowerShell smoke test with bypass so no policy change is required
powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0\smoke_test.ps1"
if errorlevel 1 (
  echo Smoke test failed
  exit /b 1
)
exit /b 0
