# アリーナ FPS をローカルサーバーで起動する（Windows / PowerShell 5.1・7 両対応）。
# ES Modules を使用しているため file:// では動作しません。静的 HTTP サーバー
# 経由で index.html を配信し、既定ブラウザで開きます。
#
# サーバーは 127.0.0.1（localhost）のみにバインドします。既定では 0.0.0.0 に
# バインドされ LAN 上の他端末から 192.168.x.x:3000 でアクセスできてしまうため、
# 外部公開を防ぐ意図的な設定です。
#
# 利用可能なランタイムを Python -> Node(npx serve) の順に自動選択します。
# $ErrorActionPreference = "Stop" は設定しません。python/npx はビルド情報を
# stderr へ書くことがあり、"Stop" だと stderr 1 行で強制終了してしまうためです。
# 代わりに $LASTEXITCODE を明示的に確認します。

$root = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$host_ = "127.0.0.1"
$port  = 3000
$url   = "http://localhost:$port"

Set-Location $root

# 実際に起動できる Python を探す。python / python3 / py の順に確認し、
# Microsoft Store のアプリ実行エイリアス（WindowsApps 配下のスタブ。実際には
# 何も実行しない）は除外したうえで --version が成功するものだけ採用する。
function Get-WorkingPython {
    foreach ($name in @('python', 'python3', 'py')) {
        $cmd = Get-Command $name -ErrorAction SilentlyContinue
        if (-not $cmd) { continue }
        if ($cmd.Source -like '*\WindowsApps\*') { continue }
        & $cmd.Source --version *> $null 2>&1
        if ($LASTEXITCODE -eq 0) { return $cmd }
    }
    return $null
}

# 少し待ってからブラウザを開く（サーバー起動を待つ）。
Start-Job -ScriptBlock {
    param($u)
    Start-Sleep -Seconds 2
    Start-Process $u
} -ArgumentList $url | Out-Null

Write-Host "アリーナ FPS を $url で起動します（Ctrl+C で停止）..." -ForegroundColor Cyan

# 1) 動作する Python があれば標準ライブラリの HTTP サーバーを使う。
$python = Get-WorkingPython
if ($python) {
    & $python.Source -m http.server $port --bind $host_
    exit $LASTEXITCODE
}

# 2) Node があれば npx serve を使う（初回は serve パッケージを自動取得）。
if (Get-Command npx -ErrorAction SilentlyContinue) {
    npx --yes serve . -l "tcp://${host_}:${port}"
    exit $LASTEXITCODE
}

Write-Host "Python も Node.js も見つかりません。どちらかをインストールしてください。" -ForegroundColor Red
exit 1