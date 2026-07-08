# Project Management MVP

ログイン付きの Kanban ボードと、ボードを操作できる AI チャットを備えた MVP です。
単一の Docker コンテナで動作します（FastAPI が API と静的フロントを配信）。

## 前提

- Docker（Windows の場合は WSL2 上の Docker で動作確認済み）
- AI 機能を使う場合はルートの `.env` に `OPENROUTER_API_KEY` を設定

ローカルでテストを実行する場合のみ、追加で以下が必要です。

- Python の `uv`（バックエンド）
- Node.js（フロントエンド）

## 起動と動作確認（Docker）

起動スクリプトがイメージのビルドとコンテナ起動をまとめて行います。

- Mac / Linux:

  ```bash
  bash scripts/start.sh
  ```

- Windows (PowerShell):

  ```powershell
  scripts\start.ps1
  ```

- Windows (WSL2 の Docker を使う場合、Git Bash などから):

  ```bash
  wsl bash -lc "cd /mnt/d/temp/pm && bash scripts/start.sh"
  ```

起動後、ブラウザで http://localhost:8000 を開きます。

1. ログイン: ユーザー名 `user` / パスワード `password`
2. Kanban ボード: カードの追加・ドラッグ移動・削除、カラム名の変更ができます。
   変更はバックエンドに保存され、リロードしても残ります。
3. AI チャット: 右下の「Assistant」ボタンでチャットを開き、例として
   「Backlog に "リリース準備" というカードを追加して」と指示すると、返信とともに
   ボードが自動で更新されます。
4. ログアウト: 右上の「Log out」でサインイン画面に戻ります。

## 停止

- Mac / Linux: `bash scripts/stop.sh`
- Windows (PowerShell): `scripts\stop.ps1`

ボード内容は名前付きボリューム `pm-data` に保存されるため、停止・再起動しても残ります。
初期状態に戻したい場合は `docker volume rm pm-data` を実行してください。

## テスト

### バックエンド（pytest）

```bash
cd backend
uv run pytest
```

AI へ実際に接続するライブテストを含める場合は、キーを読み込んでから実行します
（未設定時はスキップされます）。

```bash
cd backend
set -a && . ../.env && set +a && uv run pytest
```

### フロントエンド 単体（vitest）

```bash
cd frontend
npm install
npm run test:unit
```

### フロントエンド E2E（Playwright）

E2E はアプリが動作している状態（同一オリジン）を対象にします。先にコンテナを起動し、
別ターミナルで実行してください。チャットの E2E は実際に AI を呼びます。

```bash
# 事前にアプリを起動（http://localhost:8000）
cd frontend
npx playwright install chromium   # 初回のみ
npm run test:e2e
```

対象 URL は `PLAYWRIGHT_BASE_URL` で変更できます。

## ドキュメント

- 全体計画: `docs/PLAN.md`
- データベース設計: `docs/DATABASE.md`
- 各ディレクトリの説明: `backend/AGENTS.md`, `frontend/AGENTS.md`, `scripts/AGENTS.md`

## 処理方式

### プロンプト
当該ソリューションの以下の２つの処理方式を教えてください。
+カンバンに張るカードのD&D移動を実現している方式。
+LLMチャットで入力した指示でアプリ操作する方式

### 応答
...コードを確認しました。当該ソリューション（Kanban + AIチャットのMVP）における2つの処理方式を、実装ファイルに沿って説明します。

#### カードのD&D移動を実現している方式
ライブラリ: @dnd-kit/core を使用したクライアントサイドのドラッグ&ドロップ。バックエンドの座標計算などは使わず、フロントで完結しています。

処理の流れ

① DndContext でボード全体を囲む（KanbanBoard.tsx:151-176）
- PointerSensor に activationConstraint: { distance: 6 } を設定 → 6px 動かして初めてドラッグ開始（クリック誤爆防止）。
- 衝突判定は closestCorners（最も近い角のカラム/カードを移動先と判定）。
- ドラッグ中はカーソルに追従する見た目を DragOverlay + KanbanCardPreview で表示。

② イベントで「どれを」「どこへ」を取得（KanbanBoard.tsx:45-61）
- handleDragStart: active.id（掴んだカードID）を state に保持しプレビュー表示。
- handleDragEnd: active（掴んだカード）と over（ドロップ先）を受け取る。同一・無効なら何もしない。

③ 純粋関数 moveCard で並び替えを計算（kanban.ts:84-162）
ボードのデータ構造は「columns（順序付き cardIds の配列）＋ cards（ID→カード本体の辞書）」の正規化された形です。moveCard は cardIds 配列だけを組み替えます。
- findColumnId で移動元・移動先のカラムを特定（over が「カラム自体」か「カラム内カード」かも判定）。
- 同一カラム内 → splice で並び替え。
- 別カラム間 → 元カラムから削除し、移動先の該当位置に挿入。
- いずれもイミュータブルに新しい columns を返す。

④ 永続化（デバウンス保存）（KanbanBoard.tsx:29-35 → BoardView.tsx:25-32）
- board state が変わると onBoardChange が発火。
- BoardView が 400ms のデバウンスをかけて PUT /api/board（api.ts:38）を呼び、save_board でDBに保存（main.py:76-79）。連続操作は1回のPUTにまとまります。

▎ つまりD&Dは「dnd-kit がドラッグ操作を検知 → moveCard が配列を再計算 → React state 更新 → デバウンスで PUT 保存」という、フロント完結＋非同期保存の方式です。

#### LLMチャットの指示でアプリを操作する方式
方式の核心: LLM に「操作コマンド」を出させるのではなく、ボード全体のJSONを丸ごと書き換えて返させる（full-state replacement）方式です。関数呼び出し（tool/function calling）は使っていません。

処理の流れ

① フロントから送信（ChatSidebar.tsx:32 → api.ts:53）
- ユーザー入力とこれまでの会話履歴を POST /api/chat で送る。

② バックエンドが現在のボード＋メッセージをLLMに渡す（main.py:87-96, ai.py:65-77）
- サーバー側で保存済みボードを取得（get_board）。
- chat_about_board が以下を組み立てて OpenRouter（モデル: openai/gpt-oss-120b） に送信：
  - システムプロンプト（ai.py:51-62）
  - 現在のボードJSON（system メッセージとして注入）
  - 会話履歴 + 今回のユーザーメッセージ
- response_format={"type":"json_object"} で JSON 出力を強制。

③ プロンプトで出力フォーマットを固定（ai.py:51-62）
LLM には必ず次の2キーだけのJSONを返させる：
- reply: ユーザーへの短い自然言語の返信。
- board_update: 変更不要なら null、変更が必要なら差分ではなくボード全体（全カラム・全カード）。既存IDは維持し、新規は新IDを発番するよう指示。

④ 検証してから永続化（ai.py:79-96, main.py:94-95）
- 返ってきた文字列を json.loads。
- board_update を Pydantic の BoardData.model_validate で検証。
- 不正なら黙って破棄（board_update=None）— 壊れた提案で保存済みボードが破損しない安全策。reply は返す。
- 有効なら save_board でDBに保存。

⑤ フロントに反映（ChatSidebar.tsx:33-36 → BoardView.tsx:35-38）
- reply をチャット欄に表示。
- board_update があれば onBoardUpdate → BoardView が setBoard し、version を +1 して KanbanBoard を key 変更で再マウント（AIが差し替えたボードでUIを作り直す）。サーバー側は既に保存済みなので、フロントは再保存しません。

▎ つまりAI操作は「サーバーが現ボードJSONを文脈として渡す → LLMが全体を書き換えたJSONを返す → Pydanticで検証・保存 → フロントを再マウントして反映」という、LLMをボード状態のトランスフォーマーとして使う方式です。

#### 2つの方式の対比（要点）
| 観点 | ① D&D移動 | ② LLMチャット操作 |
|---|---|---|
| 処理場所 | フロント完結（moveCard） | バックエンド経由でLLM |
| 変更単位 | cardIds 配列の部分組み替え | ボード全体の差し替え |
| 保存 | 400msデバウンスで PUT /api/board | /api/chat 内でサーバーが直接保存 |
| UI反映 | React state 更新 | key 変更で再マウント |
| 安全策 | activation distance で誤操作防止 | Pydantic検証＋不正時は破棄 |

両者とも最終的に同じ BoardData 構造と同じDB保存経路に収束するのが、この設計のポイントです。