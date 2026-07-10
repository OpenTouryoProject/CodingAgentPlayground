# Build and start the Project Management MVP container (Windows, host-side docker CLI).
$ErrorActionPreference = "Stop"

$root = Resolve-Path (Join-Path $PSScriptRoot "..")
Set-Location $root

docker build -t pm-mvp -f backend/Dockerfile .
docker rm -f pm-mvp *> $null
docker run -d --name pm-mvp --env-file .env -p 8000:8000 -v pm-data:/app/data pm-mvp

Write-Host "Running at http://localhost:8000"
