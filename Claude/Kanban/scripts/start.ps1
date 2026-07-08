# Start the app in Docker (Windows).
$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $root

docker build -t pm-app .
docker rm -f pm-app *> $null
docker run -d --name pm-app --env-file .env -p 8000:8000 -v pm-data:/app/backend/app/data pm-app

Write-Host "Running at http://localhost:8000"
