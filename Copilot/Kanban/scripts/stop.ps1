# Stop and remove the Project Management MVP container (Windows).
# The pm-data volume (SQLite DB) is kept.
docker rm -f pm-mvp *> $null
Write-Host "Stopped."
