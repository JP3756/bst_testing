# Quick Start for macOS

## Prerequisites

1. **Install .NET 8 SDK**:
   ```bash
   # Download from: https://dotnet.microsoft.com/download/dotnet/8.0
   # Or use Homebrew:
   brew install dotnet@8
   ```

2. **Verify Installation**:
   ```bash
   dotnet --version
   # Should show: 8.0.x or higher
   ```

## First Time Setup

```bash
# 1. Clone the repository
git clone https://github.com/JP3756/bst_testing.git
cd bst_testing

# 2. Make scripts executable
chmod +x bst_backend/start-backend-with-logging.sh
chmod +x start-frontend-with-logging.sh

# 3. Restore dependencies
dotnet restore

# 4. Build the solution
dotnet build
```

## Running the Application

### Option 1: Using Shell Scripts (Recommended)

**Terminal 1 - Start Backend:**
```bash
cd bst_backend
./start-backend-with-logging.sh
```

**Terminal 2 - Start Frontend:**
```bash
# From project root
./start-frontend-with-logging.sh
```

**Terminal 3 - Watch Logs:**
```bash
cd bst_backend
tail -f backend*.log
```

### Option 2: Manual Start

**Terminal 1 - Backend:**
```bash
cd bst_backend
dotnet run
```

**Terminal 2 - Frontend:**
```bash
dotnet run --project bst_frontend.csproj
```

## Access the Application

- **Frontend UI**: http://localhost:5002
- **Backend Swagger API**: http://localhost:5000/swagger
- **Backend Health Check**: http://localhost:5000/health

## Testing the API from Terminal

```bash
# Health check
curl http://localhost:5000/health

# Insert a value
curl -X POST "http://localhost:5000/api/bst/insert?value=42"

# Get the tree
curl http://localhost:5000/api/bst/tree

# Get inorder traversal
curl http://localhost:5000/api/bst/inorder
```

## Viewing Logs

### Backend Logs (Real-time):
```bash
cd bst_backend
tail -f backend*.log
```

### Backend Logs (Last 20 lines):
```bash
cd bst_backend
tail -n 20 backend*.log
```

### Frontend Logs:
```bash
tail -f frontend.log
```

## Stopping the Application

```bash
# Press Ctrl+C in each terminal
# Or find and kill the processes:
lsof -i :5000    # Find backend process
lsof -i :5002    # Find frontend process
kill -9 <PID>    # Kill the process
```

## Troubleshooting

### Port Already in Use
```bash
# Check what's using the port
lsof -i :5000
lsof -i :5002

# Kill the process
kill -9 <PID>
```

### Permission Denied on Scripts
```bash
chmod +x *.sh
chmod +x bst_backend/*.sh
```

### HTTPS Certificate Issues
```bash
# Trust the development certificate
dotnet dev-certs https --trust
```

### Can't Access Backend from Frontend
1. Ensure backend is running: `lsof -i :5000`
2. Check backend logs: `tail -f bst_backend/backend*.log`
3. Test with curl: `curl http://localhost:5000/health`

## Development Tips

### Hot Reload
Both frontend and backend support hot reload:
```bash
dotnet watch run
```

### Running Tests
```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

### Cleaning Build Artifacts
```bash
dotnet clean
rm -rf bin obj
rm -rf bst_backend/bin bst_backend/obj
```

## Log File Locations

- **Backend**: `bst_backend/backend<YYYYMMDD>.log`
- **Frontend**: `frontend.log`

## Example Session

```bash
# Terminal 1
$ cd bst_backend
$ dotnet run
[INF] : Starting BST Backend API
[INF] : Using default listen URLs: http://localhost:5000 and https://localhost:5001
[INF] : Application started and listening.

# Terminal 2
$ dotnet run --project bst_frontend.csproj
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5002

# Terminal 3
$ cd bst_backend
$ tail -f backend*.log
2025-10-26 20:43:04.021 +08:00 [INF] : Starting BST Backend API
2025-10-26 20:43:59.110 +08:00 [INF] bst_backend: Inserted value 42 from ::1

# Browser
# Open: http://localhost:5002
```

## Useful Aliases (Optional)

Add to your `~/.zshrc` or `~/.bash_profile`:

```bash
alias bst-backend="cd ~/path/to/bst_testing/bst_backend && dotnet run"
alias bst-frontend="cd ~/path/to/bst_testing && dotnet run --project bst_frontend.csproj"
alias bst-logs="cd ~/path/to/bst_testing/bst_backend && tail -f backend*.log"
```

Then you can just run:
```bash
bst-backend   # Start backend
bst-frontend  # Start frontend
bst-logs      # Watch logs
```

---

**Everything works the same as Windows! The .NET runtime handles all platform differences automatically.** 🎉
