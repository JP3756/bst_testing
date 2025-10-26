#!/bin/bash
# Start backend with file logging enabled
cd "$(dirname "$0")"
echo "Starting BST Backend with logging..."
echo "Log file will be created at: backend.log"
dotnet run
