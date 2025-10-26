# 🌲 Binary Search Tree Visualization

A full-stack web application for visualizing and interacting with a self-balancing Binary Search Tree (AVL Tree). Built with .NET 8, Blazor WebAssembly, and ASP.NET Core.

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=.net)
![C#](https://img.shields.io/badge/C%23-239120?style=flat-square&logo=c-sharp)
![Blazor](https://img.shields.io/badge/Blazor-512BD4?style=flat-square&logo=blazor)
![License](https://img.shields.io/badge/license-MIT-blue?style=flat-square)

## 📖 Overview

This project demonstrates a complete implementation of an AVL (Adelson-Velsky and Landis) self-balancing binary search tree with an interactive web interface. The application features a Blazor WebAssembly frontend that communicates with an ASP.NET Core backend API, providing real-time tree visualization and comprehensive logging capabilities.

### ✨ Key Features

- **🎯 Interactive Tree Visualization** - Real-time visual representation of tree structure and operations
- **⚖️ AVL Self-Balancing** - Automatic tree rebalancing to maintain O(log n) operations
- **🔄 Multiple Traversals** - Support for Inorder, Preorder, Postorder, and Level-order traversals
- **🚀 RESTful API** - Complete backend API with Swagger/OpenAPI documentation
- **📝 Comprehensive Logging** - Serilog-based file and console logging with daily rotation
- **🌍 Cross-Platform** - Runs seamlessly on Windows, macOS, and Linux
- **🧪 Full Test Coverage** - Unit tests with xUnit framework

## 🖥️ Technology Stack

### Frontend
- **Blazor WebAssembly** - Modern web UI framework using C#
- **HTML5/CSS3** - Responsive design and styling
- **JavaScript Interop** - For browser-specific functionality

### Backend
- **ASP.NET Core 8.0** - High-performance web framework
- **Minimal APIs** - Lightweight API endpoints
- **Serilog** - Structured logging to file and console
- **Swagger/OpenAPI** - Interactive API documentation

### Testing
- **xUnit** - Unit testing framework
- **Coverlet** - Code coverage analysis

## 🚀 Quick Start

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or higher
- Git
- A modern web browser (Chrome, Edge, Firefox, Safari)

### Installation

```bash
# Clone the repository
git clone https://github.com/JP3756/bst_testing.git
cd bst_testing

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

Running the Application
Windows (PowerShell)

# Terminal 1 - Start Backend
cd bst_backend
dotnet run

# Terminal 2 - Start Frontend (new window)
cd ..
dotnet run --project bst_frontend.csproj

macOS/Linux

# Terminal 1 - Start Backend
cd bst_backend
dotnet run

# Terminal 2 - Start Frontend (new terminal)
cd ..
dotnet run --project bst_frontend.csproj

Access the Application
Frontend UI: http://localhost:5002
Backend Swagger API: http://localhost:5000/swagger
Health Check: http://localhost:5000/health

📚 API Documentation

Tree Operations
Endpoint	Method	Description
/api/bst/insert?value={number}	POST	Insert a single value
/api/bst/insert-bulk	POST	Bulk insert (JSON array)
/api/bst/reset	POST	Clear the entire tree
/api/bst/tree	GET	Get tree structure (JSON)

Traversals
Endpoint	Method	Description
/api/bst/inorder	GET	Inorder traversal (sorted)
/api/bst/preorder	GET	Preorder traversal
/api/bst/postorder	GET	Postorder traversal
/api/bst/levelorder	GET	Level-order traversal (BFS)
Tree Properties
Endpoint	Method	Description
/api/bst/min	GET	Get minimum value
/api/bst/max	GET	Get maximum value
/api/bst/height	GET	Get tree height
/api/bst/totalnodes	GET	Get total node count
/api/bst/leafnodes	GET	Get leaf node count

Full API documentation available at: http://localhost:5000/swagger

🏗️ Project Structure

bst_testing/
├── bst_frontend/                  # Blazor WebAssembly Frontend
│   ├── Components/                # Reusable Blazor components
│   │   ├── TreeNode.razor         # Individual node visualization
│   │   └── TreeVisualizer.razor   # Complete tree display
│   ├── Services/                  # HTTP client services
│   │   └── BstApiService.cs       # API communication layer
│   ├── Pages/                     # Application pages
│   │   └── Index.razor            # Main application page
│   └── Program.cs                 # Application entry point
│
├── bst_backend/                   # ASP.NET Core Backend API
│   ├── Services/                  
│   │   └── BstService.cs          # AVL tree implementation
│   ├── Tests/                     # Unit tests
│   │   └── BstService.Tests/      # xUnit test project
│   ├── Program.cs                 # API endpoints & configuration
│   └── backend*.log               # Serilog log files (generated)
│
├── docs/                          # Documentation
├── .gitattributes                 # Git line ending configuration
├── .gitignore                     # Git ignore rules
└── README.md                      # This file

🔧 Configuration

Port Configuration
Service	HTTP	HTTPS
Backend	5000	5001
Frontend	5002	7152

Environment Variables (Optional)

# Override backend URLs
export BST_BACKEND_URL="http://localhost:8080"

# Or specify multiple URLs
export BST_BACKEND_URLS="http://localhost:8080;https://localhost:8443"

📊 Logging
Backend Logs
The backend uses Serilog for comprehensive logging:

Location: bst_backend/backend<YYYYMMDD>.log
Format: Structured JSON-like format with timestamps
Retention: Last 7 days
Levels: Debug, Information, Warning, Error, Fatal

Example log entry:
📊 Logging
Backend Logs
The backend uses Serilog for comprehensive logging:

Location: bst_backend/backend<YYYYMMDD>.log
Format: Structured JSON-like format with timestamps
Retention: Last 7 days
Levels: Debug, Information, Warning, Error, Fatal
Example log entry:

View logs in real-time:
# Windows
Get-Content .\bst_backend\backend*.log -Wait -Tail 20

# macOS/Linux
tail -f bst_backend/backend*.log

Frontend Logs
Browser Console: Press F12 → Console tab
File Logging: Use provided scripts to capture output to frontend.log

🧪 Testing
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run tests in watch mode
dotnet watch test

🛠️ Development
Hot Reload
Both frontend and backend support hot reload for faster development:
# Backend with hot reload
cd bst_backend
dotnet watch run

# Frontend with hot reload
dotnet watch run --project bst_frontend.csproj

Clean Build
# Clean and rebuild
dotnet clean
dotnet build

# Or clean all artifacts
rm -rf **/bin **/obj
dotnet restore
dotnet build

🌍 Cross-Platform Support
This application is fully cross-platform thanks to .NET 8:

Platform	Status	Notes
✅ Windows 10/11	Fully Supported	Use PowerShell scripts
✅ macOS (Intel & Apple Silicon)	Fully Supported	Use shell scripts
✅ Linux (Ubuntu, Debian, Fedora)	Fully Supported	Use shell scripts
Platform-Specific Scripts
Windows: *.ps1 files (PowerShell)
macOS/Linux: *.sh files (Bash)
For detailed platform-specific instructions, see:

Quick Start Guide
Cross-Platform Guide
macOS Setup
🐛 Troubleshooting
Common Issues
Port Already in Use
# Windows
netstat -ano | findstr ":5000"
taskkill /F /PID <PID>

# macOS/Linux
lsof -i :5000
kill -9 <PID>

"Failed to fetch" Error

Ensure backend is running first
Verify backend is accessible: curl http://localhost:5000/health
Check no port conflicts exist
HTTPS Certificate Issues (macOS/Linux)
dotnet dev-certs https --trust

📖 Algorithm Details
AVL Tree Properties
Balance Factor: For any node, |height(left) - height(right)| ≤ 1
Time Complexity:
Insert: O(log n)
Search: O(log n)
Delete: O(log n)
Space Complexity: O(n)
Rotation Types
Left Rotation - For right-heavy nodes
Right Rotation - For left-heavy nodes
Left-Right Rotation - For left-right case
Right-Left Rotation - For right-left case
🤝 Contributing
Contributions are welcome! This is a learning project, so feel free to:

Fork the repository
Create a feature branch (git checkout -b feature/AmazingFeature)
Commit your changes (git commit -m 'Add some AmazingFeature')
Push to the branch (git push origin feature/AmazingFeature)
Open a Pull Request
📝 License
This project is available for educational purposes. Feel free to use it for learning and experimentation.

🙏 Acknowledgments
Built with .NET 8
UI powered by Blazor WebAssembly
Logging with Serilog
API documentation with Swagger/OpenAPI
📞 Support
For issues and questions:

Check the documentation
Review existing issues
Create a new issue if needed
🗺️ Roadmap
 Add delete operation with rebalancing
 Implement tree search visualization
 Add animation for rotations
 Support for other tree types (Red-Black, B-Tree)
 Export tree to image/JSON
 Step-by-step operation explanation

## 🙏 Acknowledgments

- Developed by John Paolo M. Cabaluna as part of a Project.
- Special thanks to Ezik143 for the help.
- Built with [.NET 8](https://dotnet.microsoft.com/)
- UI powered by [Blazor WebAssembly](https://dotnet.microsoft.com/apps/aspnet/web-apps/blazor)

## 👨‍💻 About the Developer

This project was created by me John Paolo M. Cabaluna as a demonstration of full-stack .NET development skills and data structure implementation.
**Skills Demonstrated:**
- Full-stack .NET development
- Data structures & algorithms
- RESTful API design
- Cross-platform development
- Test-driven development

**Connect with me:**
- 🐙 GitHub: [@JP3756](https://github.com/JP3756)
- 📧 Email: cabalunajp7@gmail.com
- 🐦 Facebook: JP Cabaluna

## 📄 About This Project

This Binary Search Tree visualization project was developed to:
- Demonstrate proficiency in .NET 8 and Blazor WebAssembly
- Showcase understanding of advanced data structures (AVL Trees)
- Illustrate full-stack development capabilities
- Practice cross-platform application development

**Project Context:**
- 🎓 University: Don Bosco Technical College - Cebu
- 📚 Course: Bachelor of Science in Information Technolgy 
- 📅 Developed: October 2025
- ⏱️ Duration: 2 weeks

📝 License

MIT License

Copyright (c) 2025 John Paolo M. Cabaluna

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
