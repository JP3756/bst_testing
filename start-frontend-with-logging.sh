#!/bin/bash
# Start frontend and capture logs to frontend.log
cd "$(dirname "$0")"
echo "Starting BST Frontend..."
echo "Logs will be written to: frontend.log"

# Run frontend and redirect output to log file
dotnet run 2>&1 | tee frontend.log
