# Cross-Platform Setup Guide (Windows, macOS, Linux)

## ✅ **Yes, this project will work on macOS!**

This is a **.NET 8.0** application which is fully cross-platform and will work on:
- ✅ Windows
- ✅ macOS (Intel and Apple Silicon)
- ✅ Linux

---

## Prerequisites

### macOS/Linux:
```bash
# Install .NET 8 SDK
# Visit: https://dotnet.microsoft.com/download/dotnet/8.0

# Verify installation
dotnet --version
# Should show: 8.0.x or higher
```

### Windows:
```powershell
# Install .NET 8 SDK (if not already installed)
# Visit: https://dotnet.microsoft.com/download/dotnet/8.0

# Verify installation
dotnet --version
```

---

## Cloning from GitHub

### On macOS/Linux:
```bash
# Clone the repository
git clone https://github.com/JP3756/bst_testing.git
cd bst_testing

# Make shell scripts executable
chmod +x bst_backend/start-backend-with-logging.sh
chmod +x start-frontend-with-logging.sh

# Restore dependencies
dotnet restore
```

### On Windows:
```powershell
# Clone the repository
git clone https://github.com/JP3756/bst_testing.git
cd bst_testing

# Restore dependencies
dotnet restore
```

---

## Running the Application

### macOS/Linux

**Terminal 1 - Backend:**
```bash
cd bst_backend
dotnet run

# Or use the shell script:
./start-backend-with-logging.sh
```

**Terminal 2 - Frontend:**
```bash
# From the root directory
dotnet run --project bst_frontend.csproj

# Or use the shell script:
./start-frontend-with-logging.sh
```

**Terminal 3 - View Backend Logs:**
```bash
cd bst_backend
tail -f backend*.log
```

### Windows

**PowerShell 1 - Backend:**
```powershell
cd bst_backend
dotnet run

# Or use the PowerShell script:
.\start-backend-with-logging.ps1
```

**PowerShell 2 - Frontend:**
```powershell
# From the root directory
dotnet run --project bst_frontend.csproj

# Or use the PowerShell script:
.\start-frontend-with-logging.ps1
```

**PowerShell 3 - View Backend Logs:**
```powershell
cd bst_backend
Get-Content backend*.log -Wait -Tail 20
```

---

## Port Configuration (Same on All Platforms)

| Application | HTTP Port | HTTPS Port |
|------------|-----------|------------|
| Backend    | 5000      | 5001       |
| Frontend   | 5002      | 7152       |

### Access URLs:
- **Frontend UI**: `http://localhost:5002`
- **Backend Swagger**: `http://localhost:5000/swagger`
- **Backend Health**: `http://localhost:5000/health`

---

## Log Files

Both platforms create the same log files:

### Backend Log:
**Location**: `bst_backend/backend<YYYYMMDD>.log`

**Format**:
```
2025-10-26 20:43:04.021 +08:00 [INF] : Starting BST Backend API
2025-10-26 20:43:59.110 +08:00 [INF] bst_backend: Inserted value 42 from ::1
```

### Frontend Log:
**Location**: `frontend.log`

**Format**:
```
[2025-10-26T20:15:35.678Z] Information: BST Frontend starting...
[2025-10-26T20:16:40.123Z] Information: Inserting value: 5
```

---

## Platform-Specific Differences

### Scripts:

| Task | Windows | macOS/Linux |
|------|---------|-------------|
| Start Backend | `.\bst_backend\start-backend-with-logging.ps1` | `./bst_backend/start-backend-with-logging.sh` |
| Start Frontend | `.\start-frontend-with-logging.ps1` | `./start-frontend-with-logging.sh` |
| View Logs | `Get-Content backend*.log -Wait` | `tail -f backend*.log` |
| Check Port | `netstat -ano \| findstr ":5000"` | `lsof -i :5000` |

### File Paths:
- Windows: Uses backslashes `\` → `bst_backend\Program.cs`
- macOS/Linux: Uses forward slashes `/` → `bst_backend/Program.cs`
- **.NET handles this automatically!** ✅

---

## Building and Testing

All commands work the same across platforms:

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build bst_backend/bst_backend.csproj

# Run tests
dotnet test

# Clean build artifacts
dotnet clean

# Restore NuGet packages
dotnet restore
```

---

## Troubleshooting

### macOS: Permission Denied on Shell Scripts
```bash
chmod +x *.sh
chmod +x bst_backend/*.sh
```

### macOS/Linux: Port Already in Use
```bash
# Find process using port 5000
lsof -i :5000

# Kill the process (replace PID with actual process ID)
kill -9 <PID>
```

### Windows: Port Already in Use
```powershell
# Find process using port 5000
netstat -ano | findstr ":5000"

# Kill the process (replace PID with actual process ID)
taskkill /F /PID <PID>
```

### HTTPS Certificate Issues (macOS/Linux)
```bash
# Trust the development certificate
dotnet dev-certs https --trust

# On Linux, you may need to manually trust the certificate
```

---

## What Works Cross-Platform ✅

- ✅ All .NET code (100% compatible)
- ✅ Serilog logging to files
- ✅ ASP.NET Core backend API
- ✅ Blazor WebAssembly frontend
- ✅ Swagger UI
- ✅ CORS configuration
- ✅ JSON serialization
- ✅ HTTP/HTTPS endpoints
- ✅ File I/O for logs
- ✅ All NuGet packages

---

## What's Different by Platform

| Feature | Windows | macOS/Linux |
|---------|---------|-------------|
| Line Endings | CRLF (`\r\n`) | LF (`\n`) |
| Shell Scripts | `.ps1` (PowerShell) | `.sh` (Bash) |
| Path Separator | `\` backslash | `/` forward slash |
| Process Management | Task Manager | Activity Monitor / top |
| Default Browser | Edge | Safari / Chrome |

**Note**: Git can handle line endings automatically with `.gitattributes` file.

---

## Recommended .gitattributes

Create this file in your repository root to handle line endings:

```gitattributes
# Auto detect text files and normalize line endings to LF
* text=auto

# Force batch scripts to always use CRLF
*.ps1 text eol=crlf
*.cmd text eol=crlf
*.bat text eol=crlf

# Force bash scripts to always use LF
*.sh text eol=lf

# Binary files
*.dll binary
*.exe binary
*.png binary
*.jpg binary
```

---

## Recommended .gitignore

Already included in the repository:
```gitignore
bin/
obj/
*.log
*.user
.vs/
.vscode/
```

---

## Testing Cross-Platform Compatibility

### On macOS (after cloning):
```bash
# 1. Navigate to project
cd bst_testing

# 2. Restore packages
dotnet restore

# 3. Build everything
dotnet build

# 4. Run tests
dotnet test

# 5. Start backend
cd bst_backend
dotnet run &
BACKEND_PID=$!

# 6. Wait for backend to start
sleep 5

# 7. Test backend API
curl http://localhost:5000/health
# Should return: OK

# 8. Test insert endpoint
curl -X POST http://localhost:5000/api/bst/insert?value=42
# Should return: 200 OK

# 9. Check logs
cat backend*.log

# 10. Start frontend (in new terminal)
cd ..
dotnet run --project bst_frontend.csproj

# 11. Open browser to http://localhost:5002

# 12. Cleanup
kill $BACKEND_PID
```

---

## Summary

### ✅ **Will it work on macOS?**
**YES!** Everything will work perfectly:

1. **Clone the repo** - Works the same
2. **Restore dependencies** - `dotnet restore` (cross-platform)
3. **Build** - `dotnet build` (cross-platform)
4. **Run** - `dotnet run` (cross-platform)
5. **Logging** - Serilog works on all platforms
6. **Browser access** - Same URLs work everywhere
7. **API endpoints** - Identical behavior

### The only differences:
- Use `.sh` scripts instead of `.ps1` on macOS/Linux
- Use `tail -f` instead of `Get-Content -Wait` for log viewing
- Use `lsof` instead of `netstat` for port checking

### Everything else is identical! 🎉

The .NET framework abstracts away all platform differences, so your C# code runs exactly the same on Windows, macOS, and Linux.
