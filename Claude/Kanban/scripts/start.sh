#!/usr/bin/env bash
# Start the app in Docker (Mac / Linux).
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"

docker build -t pm-app .
docker rm -f pm-app >/dev/null 2>&1 || true
docker run -d --name pm-app --env-file .env -p 8000:8000 \
  -v pm-data:/app/backend/app/data pm-app

echo "Running at http://localhost:8000"
