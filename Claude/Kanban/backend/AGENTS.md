# Backend

FastAPI application. Serves the JSON API under `/api/*` and the static frontend at `/`.
Packaged with `uv`. In production it runs in the Docker container; the exported frontend
(Part 3) is placed in `app/static/`.

## Layout

```
backend/
  pyproject.toml        Project + deps (managed by uv); dev group has pytest, httpx
  app/
    __init__.py
    main.py             FastAPI app: routes + SessionMiddleware + static mount + lifespan
    board.py            Pydantic BoardData/Column/Card models + initial_board() seed
    db.py               SQLite access (stdlib sqlite3): init_db, get_board, save_board
    ai.py               OpenRouter client: chat(messages, response_format?) via httpx
    static/             Static site served at / (the exported frontend in the image)
    data/               SQLite file lives here (git-ignored; created on first run)
  tests/
    test_health.py      Health + static mount
    test_auth.py        Login / logout / me
    test_board.py       Board read/write, seeding, validation, persistence
    test_ai.py          OpenRouter client (mocked + missing-key; live 2+2 when key set)
```

## Routing

- API routes are declared before the `/` static mount, so `/api/*` is matched first and
  all other paths fall through to `StaticFiles` (with `html=True`).
- `GET /api/health` -> `{"status": "ok"}`.
- `POST /api/login` (`{username, password}`) -> 200 `{user}` and sets the session cookie,
  or 401. Credentials are hardcoded (`user` / `password`) for the MVP.
- `POST /api/logout` -> clears the session.
- `GET /api/me` -> `{user}` when authenticated, else 401.
- `GET /api/board` (auth) -> the user's `BoardData`; seeds `initial_board()` on first use.
- `PUT /api/board` (auth) -> validates and replaces the user's board; returns it.
- `POST /api/chat` (auth) -> `{message, history[]}`; sends the board + conversation to the
  AI and returns `{reply, board_update}`. When `board_update` is present it is persisted.

## Database

SQLite via stdlib `sqlite3` (see `db.py` and `docs/DATABASE.md`). `init_db()` runs at
startup (lifespan) creating the file and tables if missing. The board is stored as a JSON
blob per user (`BoardData`). The DB path defaults to `app/data/app.db`, overridable with
`APP_DB_PATH` (used by tests to isolate a temp DB). In Docker the data dir is a named
volume (`pm-data`) so the board survives container recreation.

## Auth

Sessions use Starlette `SessionMiddleware` (signed, HttpOnly cookie). The secret comes
from `SESSION_SECRET` (env; a dev default is used locally). `get_current_user` is a
dependency that returns the session user or raises 401; reuse it to protect later routes.

## AI

`ai.chat(messages, response_format?)` posts to OpenRouter (`openai/gpt-oss-120b`) with
`httpx` and returns the reply text. It reads `OPENROUTER_API_KEY` from env and raises
`AIError` if it is missing.

`ai.chat_about_board(board, message, history)` builds the system prompt (board JSON +
schema instructions), calls the model in JSON mode (`response_format={"type":
"json_object"}`), and validates the reply into `AiChatResult` (`reply` +
optional `board_update: BoardData`). An invalid `board_update` is dropped so a bad
suggestion never corrupts the stored board. Structured Outputs uses JSON mode rather than
strict `json_schema` because the board's `cards` map (arbitrary id keys) is not expressible
in strict schemas.

The two live tests are skipped unless `OPENROUTER_API_KEY` is set; run them with the key
loaded from the root `.env`.

## Run and test (local)

```bash
cd backend
uv run uvicorn app.main:app --reload    # dev server on http://127.0.0.1:8000
uv run pytest                           # tests
```

## Run (Docker)

Use the repo-root scripts (`scripts/start.*`, `scripts/stop.*`), which build and run the
container defined by the root `Dockerfile`, mapping port 8000 and passing the root `.env`.

## Notes

- Environment variables (e.g. `OPENROUTER_API_KEY`) come from the root `.env`, passed to
  the container via `--env-file`.
- Keep it simple: no settings framework or extra layers until a part requires it.
