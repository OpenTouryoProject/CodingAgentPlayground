# Stop and remove the app container (Windows).
docker rm -f pm-app *> $null
Write-Host "Stopped."
