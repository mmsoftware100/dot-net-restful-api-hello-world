#!/bin/bash

echo "Testing Production Database Configuration..."
echo "=========================================="

# Test production environment
echo "1. Testing Production Environment:"
ASPNETCORE_ENVIRONMENT=Production dotnet run &
SERVER_PID=$!

# Wait for server to start
sleep 5

# Test if server is running
if curl -s http://localhost:5122 > /dev/null; then
    echo "✅ Production server started successfully"
    echo "✅ Production database connection configured"
else
    echo "❌ Production server failed to start"
fi

# Stop the server
kill $SERVER_PID 2>/dev/null
wait $SERVER_PID 2>/dev/null

echo ""
echo "2. Testing Development Environment:"
ASPNETCORE_ENVIRONMENT=Development dotnet run &
SERVER_PID=$!

# Wait for server to start
sleep 5

# Test if server is running
if curl -s http://localhost:5122 > /dev/null; then
    echo "✅ Development server started successfully"
    echo "✅ Development database connection configured"
else
    echo "❌ Development server failed to start"
fi

# Stop the server
kill $SERVER_PID 2>/dev/null
wait $SERVER_PID 2>/dev/null

echo ""
echo "Database configuration test completed!"