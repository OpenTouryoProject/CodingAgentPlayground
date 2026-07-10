# Backend

FastAPI backend for the Project Management MVP. Serves the static frontend at
`/` and the JSON API under `/api`. Packaged in Docker; managed with `uv`.

## Stack

- FastAPI + Uvicorn.
- `uv` for dependency management (`pyproject.toml`).
- Python 3.13+.

## Layout

- `app/main.py` - FastAPI app: routes, session middleware, DB lifespan, static
  files mounted at `/`.
- `app/db.py` - SQLite connection, schema init, and seed. DB is created on
  startup if missing (path `PM_DB_PATH`, default `backend/data/pm.db`).
- `app/board.py` - board read/write logic (DB rows to/from the board JSON,
  scoped to a board).
- `app/ai.py` - OpenRouter client (`ask(prompt)`), reads `OPENROUTER_API_KEY`.
- `app/static/` - the built frontend (copied in during the Docker build).
- `tests/` - pytest tests (FastAPI `TestClient`).
- `Dockerfile` - builds the image with `uv`.

## Routes

- `GET /api/health` - returns `{"status": "ok"}`.
- `POST /api/login` - body `{username, password}`; validates the hardcoded
  `user` / `password`; sets a signed session cookie. Returns `{ok: true}` or 401.
- `POST /api/logout` - clears the session.
- `GET /api/me` - returns `{"authenticated": bool}` based on the session cookie.
- `GET /api/board` - returns the current user's board as JSON (`BoardData`).
- `PATCH /api/columns/{column_id}` - body `{title}`; renames a column.
- `POST /api/cards` - body `{columnId, title, details}`; appends a card.
- `PATCH /api/cards/{card_id}` - body `{title, details}`; edits a card.
- `POST /api/cards/{card_id}/move` - body `{columnId, position}`; moves a card.
- `DELETE /api/cards/{card_id}` - deletes a card.
- `POST /api/ai/ask` - body `{prompt}`; sends the prompt to OpenRouter and returns
  `{reply}`. Requires a session; 503 if the API key is missing.
- `POST /api/ai/chat` - body `{question, history}`; sends the board JSON, chat
  history, and question to OpenRouter, applies any returned board actions via the
  board layer, and returns `{reply, board}`. Requires a session; 503 if the key is
  missing, 400 if the AI references an unknown card/column.
- `GET /` - serves the built frontend (`app/static/index.html`).

All board routes require a session and are scoped to the user's board; unknown
columns/cards (or ones on another board) return 404.

## Database

SQLite (see `docs/DATABASE.md`). Tables: `users`, `boards`, `columns`, `cards`.
Created and seeded on first run with the default `user`, one board, and the demo
columns/cards. The DB file lives under `backend/data/` (gitignored).

## AI (OpenRouter)

`app/ai.py` calls OpenRouter with the `openai` SDK (`base_url`
`https://openrouter.ai/api/v1`, model `openai/gpt-oss-120b`). The API key is read
from `OPENROUTER_API_KEY`; `app/ai.py` loads the project-root `.env` for local
runs. In Docker the key is passed at runtime via `--env-file .env` (the start
scripts do this); `.env` is never baked into the image.

- `ask(prompt)` - simple prompt/reply (connectivity check).
- `chat(board, history, question)` - sends the system prompt, the current board
  JSON, the chat history, and the question, and uses Structured Outputs
  (`response_format=AIResponse`) to get back `{reply, actions}`. `AIResponse` /
  `BoardAction` are Pydantic models; each action (`create_card`, `update_card`,
  `move_card`, `delete_card`, `rename_column`) maps to a `board.py` function via
  `board.apply_actions`, so AI edits persist through the same layer as the API.

## Auth

Signed session cookie via Starlette `SessionMiddleware`. Secret from
`SESSION_SECRET` (defaults to `dev-secret` for local runs). Credentials are
hardcoded (`user` / `password`) for the MVP.

## Run locally

```sh
cd backend
uv sync
uv run uvicorn app.main:app --reload
```

## Test

```sh
cd backend
uv run pytest
```

## Docker

Build and run via the scripts in `scripts/` (`start.sh` / `start.ps1`,
`stop.sh` / `stop.ps1`), which build the image from `backend/` and expose
port 8000.
