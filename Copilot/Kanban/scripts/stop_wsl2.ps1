# Stop and remove the app container via WSL2 (Docker Engine runs inside WSL).
# No $ErrorActionPreference = "Stop": native stderr must not abort the script.

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
# Stop the container, then stop the keepalive holder started by start_wsl2.ps1.
# Merge stderr inside bash so PowerShell never renders it as an error record.
wsl.exe --cd "$root" -- bash -lc "{ bash scripts/stop.sh; pkill -f pm-mvp-keepalive 2>/dev/null; true; } 2>&1"
