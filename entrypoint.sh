#!/bin/bash

set -e

echo "Running migrations..."
CONNECTION_STRING="$ConnectionStrings__DefaultConnection"

dotnet ./efbundle.dll --connection $CONNECTION_STRING

echo "Starting application..."
exec dotnet DataMount.Api.dll