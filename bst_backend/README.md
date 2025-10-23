# Backend helper scripts

This folder contains two simple PowerShell scripts to start and stop the backend in a separate process so you can run test HTTP requests from your terminal without accidentally killing the server.

- `start-backend.ps1` — starts the backend using `dotnet run --project` in a new process and prints the PID.
- `stop-backend.ps1` — stops any running `bst_backend` process found by name.

Usage:

```powershell
# start backend in new process
.\scripts\start-backend.ps1

# run tests from another terminal
Invoke-RestMethod -Method Post -Uri "http://localhost:5000/api/bst/insert?value=5"
Invoke-RestMethod -Uri "http://localhost:5000/api/bst/inorder"

# stop backend
.\scripts\stop-backend.ps1
```

Notes:
- Use PowerShell's `Invoke-RestMethod` for simple REST tests; `curl -X` forms may not work as expected in native PowerShell due to alias differences.
# bst_backend

Development notes for running the BST backend used by the Blazor frontend.

Requirements
- .NET 8 SDK
- PowerShell (Windows)

Run backend (PowerShell)
```powershell
cd C:\dotnet_projects\bst_backend
dotnet run
```

Smoke test (must have backend running)
```powershell
cd C:\dotnet_projects\bst_backend
.\tools\smoke_test.cmd
```

If you see HTTPS errors when running the backend because a dev certificate is missing, either run `dotnet dev-certs https --trust` or run the backend HTTP-only. The project is configured to listen on `http://localhost:5000` by default.

Running tests
---------------
There is a lightweight console test runner that verifies the `BstService` behavior without needing xUnit. To run it:

```powershell
cd C:\dotnet_projects\bst_backend\tests-runner
dotnet run
```

This runner prints `All tests passed.` when the BST methods behave as expected.
