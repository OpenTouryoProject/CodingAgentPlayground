# Reset the app database via WSL2: stop the app, delete the pm-data volume
# (the persisted SQLite DB), then start again. On the next startup the initial
# board is re-seeded. Destructive: this erases all board data.
#
# Usage:
#   .\scripts\reset_db_wsl2.ps1          # asks for confirmation
#   .\scripts\reset_db_wsl2.ps1 -Force   # no prompt
#
# No $ErrorActionPreference = "Stop": native stderr must not abort the script.
param([switch]$Force)

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

if (-not $Force) {
    Write-Host "pm-data ボリューム（DB）を削除し、カンバンを初期状態に戻します。" -ForegroundColor Yellow
    $ans = Read-Host "続行しますか? [y/N]"
    if ($ans -ne 'y' -and $ans -ne 'Y') {
        Write-Host "中止しました。"
        return
    }
}

# Stop the container and the keepalive holder so the volume is not in use.
wsl.exe --cd "$root" -- bash -lc "{ bash scripts/stop.sh; pkill -f pm-mvp-keepalive 2>/dev/null; true; } 2>&1"

# Remove the persisted database volume (missing volume is fine = already clean).
wsl.exe --cd "$root" -- bash -lc "docker volume rm pm-data 2>&1 || echo 'pm-data ボリュームはありません（既にクリーンです）'"

# Start again: rebuild if needed, recreate the volume, keepalive, open browser.
Write-Host "初期化しました。再起動します..." -ForegroundColor Green
& (Join-Path $PSScriptRoot "start_wsl2.ps1")
