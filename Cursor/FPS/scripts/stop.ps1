# アリーナ FPS のローカルサーバーを停止する（Windows / PowerShell 5.1・7 両対応）。
# start.ps1 は通常フォアグラウンドで動作し Ctrl+C で止まりますが、別ターミナルや
# バックグラウンドで残ってしまったサーバーを止めるための補助スクリプトです。
# ポート 3000 で待ち受けているプロセス（python http.server / node serve）を停止します。

$port = 3000

$conns = Get-NetTCPConnection -LocalPort $port -State Listen -ErrorAction SilentlyContinue
if (-not $conns) {
    Write-Host "ポート $port で待ち受けているサーバーはありません。" -ForegroundColor Yellow
    exit 0
}

$procIds = $conns.OwningProcess | Sort-Object -Unique
foreach ($procId in $procIds) {
    $proc = Get-Process -Id $procId -ErrorAction SilentlyContinue
    $name = if ($proc) { $proc.ProcessName } else { "?" }
    try {
        Stop-Process -Id $procId -Force -ErrorAction Stop
        Write-Host "停止しました: PID $procId ($name)" -ForegroundColor Green
    } catch {
        Write-Host "PID $procId ($name) の停止に失敗しました: $_" -ForegroundColor Red
    }
}