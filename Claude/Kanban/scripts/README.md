# scripts — 起動/停止スクリプトと WSL2 対応メモ

アプリを Docker で起動・停止するスクリプト群です。

## スクリプト一覧

| スクリプト | 対象 | 説明 |
|---|---|---|
| `start.sh` / `stop.sh` | Mac / Linux（および WSL2 内の bash） | Docker でビルド・起動 / 停止 |
| `start.ps1` / `stop.ps1` | Windows（**ホスト側に docker CLI がある**環境） | 同上。`docker` コマンドを直接呼ぶ |
| `start_wsl2.ps1` / `stop_wsl2.ps1` | Windows（**Docker が WSL2 内**にある環境） | PowerShell から **WSL 経由**で `start.sh` / `stop.sh` を実行 |

`start` はイメージ `pm-app` をビルドし、既存コンテナを削除してから新規起動します。ポート 8000 を公開し、ルートの `.env` を `--env-file` で渡し、名前付きボリューム `pm-data` を SQLite データディレクトリにマウントします（ボード内容がコンテナ再作成をまたいで永続化）。起動後は http://localhost:8000 で利用できます。`stop` はコンテナを削除します（`pm-data` は保持）。

---

## Windows で `.\scripts\start.ps1` が失敗する場合

### 症状

```
docker : 用語 'docker' は、コマンドレット、関数、スクリプト ファイル、または操作可能な
プログラムの名前として認識されません。
    + docker build -t pm-app .
```

### 原因

**Docker が Windows ホスト側ではなく WSL2（Ubuntu ディストロ）の中にインストールされている**ため、Windows の PowerShell から `docker` を直接呼ぶ `start.ps1` はコマンドを見つけられません。

確認方法:

```powershell
docker --version                 # ホスト側: 見つからない
wsl bash -lc "docker --version"  # WSL 内: バージョンが表示される
```

### 対処

WSL 経由で Docker を叩く **`start_wsl2.ps1` / `stop_wsl2.ps1`** を使います。

```powershell
.\scripts\start_wsl2.ps1     # WSL 経由でビルド&起動 → ブラウザで http://localhost:8000 を開く
.\scripts\stop_wsl2.ps1      # 停止
```

`start_wsl2.ps1` の動作:

1. WSL 内で `docker info` が通るか確認（未起動なら案内メッセージを表示して終了）。
2. `wsl --cd <repo> -- bash -lc "bash scripts/start.sh"` でビルド&起動。
3. `http://localhost:8000` をブラウザで開く（localhost が繋がらない場合の代替として WSL の IP も表示）。

---

## ハマりどころと解決

### 1. `start.sh` が `set: pipefail: invalid option name` で失敗する

**原因**: シェルスクリプトの改行コードが **CRLF（Windows 改行）**になっていると、bash 実行時に行末の `\r` が引数に紛れ込み、`set -euo pipefail` の `pipefail` が不正なオプション名として弾かれます。Windows の Git（`core.autocrlf=true`）でチェックアウトすると `.sh` が CRLF 化されるのが典型的な原因です。

確認方法（WSL 内）:

```bash
file scripts/start.sh   # "with CRLF line terminators" と出れば CRLF
```

**解決**: リポジトリ直下（`Claude/Kanban/`）に **`.gitattributes`** を追加し、`.sh` を常に LF でチェックアウトさせます。

```
# .gitattributes
*.sh text eol=lf
```

既にチェックアウト済みのファイルは一度 LF へ変換します:

```bash
sed -i 's/\r$//' scripts/*.sh          # ディスク上の CRLF を除去
git add --renormalize scripts          # インデックスも LF に正規化
```

> 注意: `git add --renormalize` は指定パスに限定して実行してください（リポジトリ全体に対して行うと、無関係なファイルまで正規化対象になります）。

### 2. サインイン時に「Failed to fetch」／`localhost:8000` に繋がらない

ページは開けるのにサインインで「Failed to fetch」になる、あるいは Windows の `http://localhost:8000` が「接続が拒否されました」やタイムアウトになることがあります。

切り分け結果、**localhost 転送も API コードも正常**で、繋がらない主因は次の 2 つでした。

- **WSL2 の VM がアイドルで停止し、コンテナごと落ちている**（ページ表示時は生きていても、サインインする頃には落ちている）。ブラウザ操作は Windows 側の localhost 転送を使うだけで **WSL セッションを維持しない**ため、操作していても VM は落ちます。
- コンテナに再起動ポリシーが無く、VM/Docker 復帰時に自動復活しない。

**解決策（このリポジトリ内で完結。グローバルな `.wslconfig` は不要）**:

- **`start_wsl2.ps1` が「維持プロセス（keepalive holder）」を常駐起動**します。コンテナが動いている間だけ WSL セッションを 1 本保持して VM のアイドル停止を防ぎ、コンテナ停止後（`stop_wsl2.ps1` 実行後）は約 20 秒で自動終了します。これにより、ブラウザだけで操作していてもコンテナが落ちません。
- `start.sh` の `docker run` に **`--restart unless-stopped`** を付与済み。dockerd が復帰するたびコンテナが自動で復活します（自己回復）。
- この WSL は **systemd 有効 + `docker.service` 自動起動**のため、ディストロが起動すれば dockerd も自動で立ち上がります。

> 検証: `start_wsl2.ps1` 実行後、WSL に一切触れず 80 秒放置してもコンテナは `Up` のままで、Windows の `localhost:8000` から index 200・ログイン成功を確認済み。

**それでも localhost が繋がらない場合**の代替:

- WSL の IP で開く: `http://<WSL_IP>:8000`（IP は `wsl hostname -I` の先頭。再起動で変わります）。`start_wsl2.ps1` が起動時に表示します。
- localhost 転送のリレーをリセット: 他の WSL 作業を閉じてから `wsl --shutdown` → 再度 `start_wsl2.ps1`。

> `.wslconfig` の `networkingMode=mirrored` でも解決できますが、**全ディストロに影響するグローバル設定**のため、ここでは採用していません。

### 3. `start_wsl2.ps1` が `WARNING: No blkio throttle...` などで即停止する

**症状**: `wsl.exe --cd ... -- bash -lc "docker info"` の行で以下のように停止する。

```
wsl.exe : WARNING: No blkio throttle.read_bps_device support
    + CategoryInfo          : NotSpecified: (...) [], RemoteException
    + FullyQualifiedErrorId : NativeCommandError
```

**原因**: PowerShell で `$ErrorActionPreference = "Stop"` を設定していると、ネイティブコマンド（`wsl.exe` / `docker`）が **stderr に 1 行でも出力しただけで**「NativeCommandError」として即座に終了します。`docker info` の無害な警告や、`docker build` の進捗（BuildKit は進捗を stderr に出す）がこれに該当します。

**解決**: `start_wsl2.ps1` / `stop_wsl2.ps1` では `$ErrorActionPreference = "Stop"` を**使わず**、代わりに各 `wsl.exe` 呼び出し後に `$LASTEXITCODE` を明示チェックしています。加えて、確認だけの `docker info` は bash 側で出力を握りつぶし（`docker info > /dev/null 2>&1`）、ビルドは `2>&1` で stderr を標準出力へマージして通常表示にしています。

### 4. スクリプト実行時に日本語メッセージが文字化けする（例: `縺檎ｹ九′...`）

**原因**: **Windows PowerShell 5.1** は、BOM の無い `.ps1` を UTF-8 ではなくシステム ANSI コードページ（日本語環境では CP932）として読み込みます。そのためファイル内の日本語文字列リテラルが誤解釈されて化けます（動作自体には影響しません）。

**解決**: `start_wsl2.ps1` / `stop_wsl2.ps1` を **UTF-8 (BOM 付き)** で保存済みです。日本語を含む `.ps1` を編集する際は、BOM 付き UTF-8 を維持してください（BOM が外れると再発します）。PowerShell 7 (`pwsh`) は BOM 無し UTF-8 でも正しく読めます。

---

## 前提と補足

- Docker が動作していること（このリポジトリでは WSL2 内の Docker Engine を利用）。
- AI チャット機能を使う場合は、ルートの `.env` に `OPENROUTER_API_KEY` を設定します。未設定でもログイン・カンバン操作・カードの D&D は動作します（チャット送信時のみ 503）。
- ログイン: ユーザー名 `user` / パスワード `password`。
