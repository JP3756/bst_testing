# Quick Start Guide - BST Application

## Problem Fixed ✅

The "Failed to fetch" error was caused by:
1. **Backend not running** - The API server needs to be started first
2. **Port conflict** - Frontend and backend were both trying to use port 5001

## How to Start the Application

### Step 1: Start the Backend (API Server)
```powershell
# Open a PowerShell terminal
cd C:\dotnet_projects\bst_frontend\bst_backend
dotnet run
```

**Backend will listen on:**
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

**You'll see output like:**
```
[INF] : Starting BST Backend API
[INF] : Using default listen URLs: http://localhost:5000 and https://localhost:5001
[INF] : Application started and listening.
```

### Step 2: Start the Frontend (Blazor UI)
```powershell
# Open a NEW PowerShell terminal
cd C:\dotnet_projects\bst_frontend
dotnet run
```

**Frontend will listen on:**
- HTTP: `http://localhost:5002`

### Step 3: Open in Browser
Navigate to: **http://localhost:5002**

## Testing the Application

1. Enter a number in the input field
2. Click "Insert"
3. The number will be sent to the backend API
4. Check the tree visualization

## Viewing Logs

### Backend Logs:
```powershell
# Real-time log viewing
cd C:\dotnet_projects\bst_frontend\bst_backend
Get-Content backend*.log -Wait -Tail 20
```

**What you'll see in logs:**
- Application startup
- Each insert operation with value and source IP
- Bulk operations
- Any errors

### Frontend Logs:
Frontend logs to browser console:
1. Press F12 to open DevTools
2. Go to Console tab
3. See application logs

## Swagger API Documentation

With the backend running, open: **http://localhost:5000/swagger**

You can test all API endpoints directly from Swagger, and all operations will be logged to `backend20251026.log`

## Troubleshooting

### "Failed to fetch" Error
- ✅ **FIXED**: Make sure backend is running first
- ✅ **FIXED**: Frontend now uses port 5002 (no conflict)
- The backend CORS is configured to allow all origins

### Backend won't start
```powershell
# Check if ports are in use
netstat -ano | findstr ":5000"
netstat -ano | findstr ":5001"

# Kill conflicting process if needed
# Find the PID from netstat output
taskkill /F /PID <process_id>
```

### Frontend won't start
```powershell
# Check if port 5002 is in use
netstat -ano | findstr ":5002"
```

## Quick Scripts

### Start Backend with Logging
```powershell
.\bst_backend\start-backend-with-logging.ps1
```

### Start Frontend with Logging
```powershell
.\start-frontend-with-logging.ps1
```

## Ports Summary

| Application | HTTP Port | HTTPS Port |
|------------|-----------|------------|
| Backend    | 5000      | 5001       |
| Frontend   | 5002      | 7152       |

## Example Session

```powershell
# Terminal 1 - Backend
PS> cd C:\dotnet_projects\bst_frontend\bst_backend
PS> dotnet run
[INF] : Starting BST Backend API
[INF] : Application started and listening.

# Terminal 2 - Frontend  
PS> cd C:\dotnet_projects\bst_frontend
PS> dotnet run
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5002

# Terminal 3 - Watch logs
PS> cd C:\dotnet_projects\bst_frontend\bst_backend
PS> Get-Content backend*.log -Wait
2025-10-26 20:43:04.021 [INF] : Starting BST Backend API
2025-10-26 20:43:59.110 [INF] bst_backend: Inserted value 42 from ::1
```

Now open `http://localhost:5002` in your browser and start inserting numbers!
