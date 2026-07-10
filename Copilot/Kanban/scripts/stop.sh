#!/usr/bin/env bash
# Stop and remove the Project Management MVP container (Mac / Linux, and WSL2 bash).
# The pm-data volume (SQLite DB) is kept.
set -euo pipefail

docker rm -f pm-mvp >/dev/null 2>&1 || true
echo "Stopped."
