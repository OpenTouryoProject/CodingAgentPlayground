# Start the app in Docker via WSL2 (Windows host has no docker CLI).
# Docker Engine runs inside WSL2, so we drive the build/run through WSL.
#
# Note: we intentionally do NOT set $ErrorActionPreference = "Stop". Native
# commands (wsl.exe/docker) write warnings and build progress to stderr, and
# under "Stop" PowerShell turns any stderr line into a terminating error. We
# check $LASTEXITCODE explicitly instead.

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

# Docker must be reachable inside WSL (swallow docker's output/warnings; only
# the exit code matters here).
wsl.exe --cd "$root" -- bash -lc "docker info > /dev/null 2>&1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "WSL 内で Docker に接続できません。WSL のターミナルで 'sudo service docker start' を実行してから再試行してください。" -ForegroundColor Red
    exit 1
}

# Build the image and (re)start the container inside WSL. Merge stderr into
# stdout *inside bash* (not with a PowerShell `2>&1`) so PowerShell never sees
# a native stderr stream and thus never renders BuildKit progress as error
# records. BUILDKIT_PROGRESS=plain avoids the \r in-place updates that would
# otherwise render as a garbled staircase when captured.
wsl.exe --cd "$root" -- bash -lc "BUILDKIT_PROGRESS=plain bash scripts/start.sh 2>&1"
if ($LASTEXITCODE -ne 0) {
    Write-Host "起動に失敗しました。上のログを確認してください。" -ForegroundColor Red
    exit 1
}

# Keep the WSL2 VM alive while the container runs. WSL2 shuts the VM down when
# no session is attached (browser traffic over localhost forwarding does NOT
# count), which would kill the container and cause "Failed to fetch" in the UI.
# This detached holder keeps a WSL session open and self-exits ~20s after the
# container stops (e.g. after stop_wsl2.ps1). Only one holder runs at a time.
$holder = 'pkill -f "pm-mvp-keepalive" 2>/dev/null; ' +
          '( exec -a pm-mvp-keepalive bash -c ' +
          '"while docker inspect -f {{.State.Running}} pm-mvp 2>/dev/null | grep -q true; do sleep 20; done" )'
Start-Process -WindowStyle Hidden wsl.exe -ArgumentList @('--cd', "$root", '--', 'bash', '-lc', $holder)

# Open the standard URL. If Windows->WSL localhost forwarding is unhealthy,
# fall back to the WSL IP printed below.
$wslIp = ((wsl.exe hostname -I) -split '\s+' | Where-Object { $_ })[0]
# start.sh already printed "Running at http://localhost:8000"; add the fallback.
Write-Host "(localhost が繋がらない場合は http://${wslIp}:8000 を使用してください)"
Start-Process "http://localhost:8000"
