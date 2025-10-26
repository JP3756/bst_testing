# Binary Search Tree (BST) Visualization Application

A full-stack .NET 8 application with a Blazor WebAssembly frontend and ASP.NET Core backend API for visualizing and interacting with a self-balancing (AVL) Binary Search Tree.

## 🌟 Features

- **Interactive Tree Visualization**: Real-time visual representation of BST structure
- **AVL Self-Balancing**: Automatic tree balancing for optimal performance
- **Multiple Traversals**: Inorder, Preorder, Postorder, and Level-order traversals
- **RESTful API**: Complete backend API with Swagger documentation
- **Comprehensive Logging**: Serilog-based file and console logging
- **Cross-Platform**: Runs on Windows, macOS, and Linux

## 🖥️ Platform Support

| Platform | Status | .NET Version |
|----------|--------|--------------|
| Windows 10/11 | ✅ Fully Supported | .NET 8.0 |
| macOS (Intel & Apple Silicon) | ✅ Fully Supported | .NET 8.0 |
| Linux (Ubuntu, Debian, Fedora) | ✅ Fully Supported | .NET 8.0 |

## 📋 Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- Git
- A modern web browser (Chrome, Firefox, Edge, Safari)

### Platform-Specific Installation:

**Windows:**
```powershell
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0
# Or use winget:
winget install Microsoft.DotNet.SDK.8
```

**macOS:**
```bash
# Download from: https://dotnet.microsoft.com/download/dotnet/8.0
# Or use Homebrew:
brew install dotnet@8
```

**Linux (Ubuntu/Debian):**
```bash
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 8.0
```

## 🚀 Quick Start

### 1. Clone the Repository

```bash
git clone https://github.com/JP3756/bst_testing.git
cd bst_testing
```

### 2. Make Scripts Executable (macOS/Linux only)

```bash
chmod +x bst_backend/start-backend-with-logging.sh
chmod +x start-frontend-with-logging.sh
```

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Run the Application

**Windows (PowerShell):**
```powershell
# Terminal 1 - Backend
cd bst_backend
dotnet run

# Terminal 2 - Frontend (new terminal)
cd ..
dotnet run --project bst_frontend.csproj
```

**macOS/Linux (Bash/Zsh):**
```bash
# Terminal 1 - Backend
cd bst_backend
dotnet run

# Terminal 2 - Frontend (new terminal)
cd ..
dotnet run --project bst_frontend.csproj
```

### 6. Access the Application

- **Frontend UI**: http://localhost:5002
- **Backend Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health

## 📁 Project Structure

```
bst_testing/
├── bst_frontend/              # Blazor WebAssembly frontend
│   ├── Components/            # Blazor components
│   │   ├── TreeNode.razor     # Tree node visualization
│   │   └── TreeVisualizer.razor
│   ├── Services/              # API client services
│   │   └── BstApiService.cs
│   ├── Program.cs             # Frontend entry point
│   └── bst_frontend.csproj
│
├── bst_backend/               # ASP.NET Core backend API
│   ├── Services/              
│   │   └── BstService.cs      # AVL tree implementation
│   ├── Tests/                 # Unit tests
│   ├── Program.cs             # Backend entry point & API endpoints
│   ├── bst_backend.csproj
│   └── backend*.log           # Serilog log files (generated)
│
├── .gitignore                 # Git ignore rules
├── .gitattributes             # Line ending configuration
├── README.md                  # This file
├── QUICK_START.md             # Quick start guide
├── CROSS_PLATFORM.md          # Cross-platform compatibility guide
└── README_MACOS.md            # macOS-specific guide
```

## 🔧 Configuration

### Ports

| Service | HTTP | HTTPS |
|---------|------|-------|
| Backend | 5000 | 5001 |
| Frontend | 5002 | 7152 |

### Environment Variables (Optional)

```bash
# Override backend URLs
export BST_BACKEND_URL="http://localhost:8080"
# Or multiple URLs
export BST_BACKEND_URLS="http://localhost:8080;https://localhost:8443"
```

## 📊 API Endpoints

### Core Operations
- `POST /api/bst/insert?value={number}` - Insert a value
- `POST /api/bst/insert-bulk` - Bulk insert (JSON array)
- `POST /api/bst/reset` - Clear the tree
- `GET /api/bst/tree` - Get tree structure (JSON)

### Traversals
- `GET /api/bst/inorder` - Inorder traversal
- `GET /api/bst/preorder` - Preorder traversal
- `GET /api/bst/postorder` - Postorder traversal
- `GET /api/bst/levelorder` - Level-order traversal

### Tree Properties
- `GET /api/bst/min` - Get minimum value
- `GET /api/bst/max` - Get maximum value
- `GET /api/bst/height` - Get tree height
- `GET /api/bst/totalnodes` - Get node count
- `GET /api/bst/leafnodes` - Get leaf node count

### Health
- `GET /health` - Health check endpoint

**Full API documentation**: http://localhost:5000/swagger

## 📝 Logging

### Backend Logs
- **Location**: `bst_backend/backend<YYYYMMDD>.log`
- **Format**: Timestamp, log level, source, message
- **Retention**: Last 7 days

**View logs:**
```bash
# Windows
Get-Content .\bst_backend\backend*.log -Wait -Tail 20

# macOS/Linux
tail -f bst_backend/backend*.log
```

### Frontend Logs
- **Location**: `frontend.log` (when using logging scripts)
- **Browser Console**: Press F12 → Console tab

## 🧪 Testing

```bash
# Run all tests
dotnet test

# Run tests in a specific project
dotnet test bst_backend/Tests/BstService.Tests/BstService.Tests.csproj

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## 🛠️ Development

### Hot Reload
```bash
# Backend with hot reload
cd bst_backend
dotnet watch run

# Frontend with hot reload
dotnet watch run --project bst_frontend.csproj
```

### Clean Build
```bash
dotnet clean
dotnet build
```

## 🐛 Troubleshooting

### Port Already in Use

**Windows:**
```powershell
netstat -ano | findstr ":5000"
taskkill /F /PID <PID>
```

**macOS/Linux:**
```bash
lsof -i :5000
kill -9 <PID>
```

### "Failed to fetch" Error
1. ✅ Ensure backend is running first
2. ✅ Check backend is accessible: `curl http://localhost:5000/health`
3. ✅ Verify no port conflicts

### HTTPS Certificate Issues (macOS/Linux)
```bash
dotnet dev-certs https --trust
```

## 📚 Additional Documentation

- **[Quick Start Guide](QUICK_START.md)** - Detailed startup instructions
- **[Cross-Platform Guide](CROSS_PLATFORM.md)** - Platform-specific details
- **[macOS Guide](README_MACOS.md)** - macOS-specific instructions
- **[Logging Setup](LOGGING_SETUP.md)** - Logging configuration

## 🔐 Security Notes

- CORS is configured to allow all origins (development only)
- HTTPS certificates are self-signed (development only)
- For production, configure proper certificates and CORS policies

## 📄 License

This project is for educational purposes.

## 🤝 Contributing

This is a learning project. Feel free to fork and experiment!

## ✨ Technology Stack

- **Frontend**: Blazor WebAssembly, C#, HTML/CSS
- **Backend**: ASP.NET Core 8.0, Minimal APIs
- **Logging**: Serilog
- **API Documentation**: Swagger/OpenAPI
- **Testing**: xUnit
- **Data Structure**: AVL Tree (self-balancing BST)

## 📞 Support

For platform-specific issues:
- Windows: See [QUICK_START.md](QUICK_START.md)
- macOS: See [README_MACOS.md](README_MACOS.md)
- Linux: See [CROSS_PLATFORM.md](CROSS_PLATFORM.md)

---

**Built with ❤️ using .NET 8 - Write once, run anywhere!**
