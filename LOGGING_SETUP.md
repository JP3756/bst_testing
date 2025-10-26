# Logging Setup for BST Project

## Backend Logging (backend.log)

The backend now uses **Serilog** for comprehensive logging to both console and file.

### Features:
- **Console logging**: Formatted output with timestamps
- **File logging**: Logs written to `backend.log` with daily rolling
- **Swagger integration**: All API calls and operations are logged
- **Retention**: Keeps last 7 days of log files

### Log File Location:
```
bst_backend/backend<YYYYMMDD>.log
```

### Log Format:
```
YYYY-MM-DD HH:mm:ss.fff zzz [LEVEL] SourceContext: Message
```

### What Gets Logged:
- Application startup and shutdown
- URL configuration
- API requests (insert, bulk-insert, etc.)
- Errors and exceptions
- Remote IP addresses for requests

### To Start Backend with Logging:
```powershell
cd bst_backend
dotnet run
```

Or use the convenience script:
```powershell
.\bst_backend\start-backend-with-logging.ps1
```

### View Logs:
```powershell
Get-Content .\bst_backend\backend*.log -Wait
```

---

## Frontend Logging (frontend.log)

The frontend (Blazor WebAssembly) logs to browser console and can be captured to file when running via dotnet CLI.

### Features:
- **Browser console logging**: View in browser DevTools
- **File capture**: When using the startup script
- **Configurable log levels**

### Log File Location:
```
frontend.log
```

### To Start Frontend with Logging:
```powershell
.\start-frontend-with-logging.ps1
```

This script will:
1. Start the frontend application
2. Capture all console output to `frontend.log`
3. Display logs in terminal (via Tee-Object)

### View Browser Logs:
1. Open browser DevTools (F12)
2. Go to Console tab
3. Filter for your application logs

---

## Viewing Swagger with Backend Logs

1. Start the backend:
   ```powershell
   cd bst_backend
   dotnet run
   ```

2. Open Swagger UI:
   ```
   http://localhost:5000/swagger
   ```

3. Make API calls through Swagger - all will be logged to `backend.log`

4. View logs in real-time:
   ```powershell
   Get-Content .\bst_backend\backend*.log -Wait -Tail 20
   ```

---

## Configuration

### Backend Log Levels:
Edit `bst_backend/Program.cs`:
```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()  // Change this for different log levels
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    ...
```

### Frontend Log Levels:
Edit `Program.cs`:
```csharp
builder.Logging.SetMinimumLevel(LogLevel.Information);  // Change as needed
```

---

## Troubleshooting

### Backend log not created:
- Ensure backend is running: `Get-Process dotnet`
- Check write permissions in bst_backend folder
- Verify Serilog.AspNetCore package is installed

### Frontend log empty:
- Frontend logs primarily to browser console
- Use startup script to capture output
- Check browser DevTools Console tab

---

## Example Log Entries

### Backend (backend.log):
```
2025-10-26 20:15:30.123 +00:00 [INF] bst_backend: Using default listen URLs: http://localhost:5000 and https://localhost:5001
2025-10-26 20:15:31.456 +00:00 [INF] bst_backend: Application started and listening.
2025-10-26 20:16:05.789 +00:00 [INF] bst_backend: Inserted value 42 from 127.0.0.1
2025-10-26 20:16:10.234 +00:00 [INF] bst_backend: Bulk inserted 5 values from 127.0.0.1
```

### Frontend (Console):
```
[2025-10-26T20:15:35.678Z] Information: BST Frontend starting...
[2025-10-26T20:15:36.012Z] Information: HttpClient configured with base address http://localhost:5000
```
