#!/usr/bin/env sh
# Build and start the Project Management MVP container (Mac / Linux).
set -e
cd "$(dirname "$0")/.."
docker build -t pm-mvp -f backend/Dockerfile .
docker run -d --rm --name pm-mvp --env-file .env -p 8000:8000 pm-mvp
echo "Running at http://localhost:8000"
