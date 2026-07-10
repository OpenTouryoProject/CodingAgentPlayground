# Build and start the Project Management MVP container (Windows).
$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")
docker build -t pm-mvp -f backend/Dockerfile .
docker run -d --rm --name pm-mvp --env-file .env -p 8000:8000 pm-mvp
Write-Host "Running at http://localhost:8000"
