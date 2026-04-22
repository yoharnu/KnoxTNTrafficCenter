#!/bin/bash
echo "Starting application..."
dotnet run --project KnoxTrafficCenter/KnoxTrafficCenter.csproj > app_output.log 2>&1 &
APP_PID=$!

# Wait for app to start
sleep 5

echo "Measuring time for 10 requests to /api/alerts/incidents..."
time for i in {1..10}; do curl -s -o /dev/null http://localhost:5260/api/alerts/incidents; done

echo "Measuring time for 10 requests to /api/alerts/weather..."
time for i in {1..10}; do curl -s -o /dev/null http://localhost:5260/api/alerts/weather; done

echo "Stopping application..."
kill $APP_PID
