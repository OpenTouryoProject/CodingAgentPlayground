#!/usr/bin/env bash
# Build and start the Project Management MVP container (Mac / Linux, and WSL2 bash).
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

docker build -t pm-mvp -f backend/Dockerfile .
docker rm -f pm-mvp >/dev/null 2>&1 || true
docker run -d --name pm-mvp --restart unless-stopped --env-file .env -p 8000:8000 \
  -v pm-data:/app/data pm-mvp

echo "Running at http://localhost:8000"
