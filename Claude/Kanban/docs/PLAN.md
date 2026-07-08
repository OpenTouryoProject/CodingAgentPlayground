# Project Management MVP - Detailed Plan

This plan expands each part of the project into a checklist with tests and success
criteria. Work proceeds one part at a time; each part must meet its success criteria
before the next begins.

## Confirmed decisions

- Frontend serving: Next.js static export (`output: 'export'`), served by FastAPI
  `StaticFiles` at `/`. Single Docker container. No SSR.
- Auth: hardcoded `user` / `password`; on success the server issues a session stored in an
  HttpOnly cookie. Logout clears it. Multiple users supported in the schema for the future.
- Database: SQLite. `users` table plus a `boards` table storing the whole board as a JSON
  blob in one column (`BoardData` shape). Created automatically if it does not exist.
- AI: OpenRouter, model `openai/gpt-oss-120b`, `OPENROUTER_API_KEY` from root `.env`.
- Package manager for Python (in Docker): `uv`.

## Conventions

- Backend package: `backend/app/` (FastAPI app), tests in `backend/tests/`.
- Run backend tests with `uv run pytest`. Run frontend unit tests with `npm run test:unit`
  and e2e with `npm run test:e2e`.
- Keep it simple: no features beyond what each part requires, no speculative abstraction.
- No emojis anywhere. Keep docs minimal.

---

## Part 1: Plan (this document)

- [x] Enrich this document with per-part checklists, tests, and success criteria.
- [x] Create `frontend/AGENTS.md` describing the existing frontend code.
- [ ] User reviews and approves the plan.

Tests / verification:
- The user confirms the plan and the confirmed decisions above.

Success criteria:
- This document and `frontend/AGENTS.md` exist and are approved by the user.

---

## Part 2: Scaffolding (Docker + FastAPI + scripts)

Goal: a running container that serves a placeholder static page at `/` and exposes a
working API endpoint. No frontend build yet.

- [x] Create `backend/pyproject.toml` (managed by `uv`) with FastAPI and Uvicorn.
- [x] Create `backend/app/main.py` with a FastAPI app:
  - [x] `GET /api/health` returns `{"status": "ok"}`.
  - [x] Mount `StaticFiles` at `/` serving `backend/app/static/` (placeholder
        `index.html` saying hello world).
- [x] Add a placeholder `backend/app/static/index.html` that shows "hello world" and
      calls `GET /api/health`, displaying the result on the page.
- [x] Write `backend/AGENTS.md` describing the backend layout and how to run it.
- [x] Create `Dockerfile` (root) that uses `uv` to install deps and runs Uvicorn.
- [x] Create `.dockerignore`.
- [x] Write start/stop scripts in `scripts/`:
  - [x] `start.sh` / `stop.sh` (Mac + Linux)
  - [x] `start.ps1` / `stop.ps1` (Windows)
  - [x] Scripts build/run and stop the Docker container, mapping a port and passing `.env`.
- [x] Add `backend/tests/test_health.py` using FastAPI `TestClient`.

Tests:
- [x] `uv run pytest` passes (`GET /api/health` returns 200 and expected JSON).
- [x] Container builds and runs; `curl http://localhost:8000/api/health` returns ok.
      (Verified via WSL2 Docker; `scripts/start.sh` builds+runs, `scripts/stop.sh` stops.)
- [x] Visiting `/` (local Uvicorn) shows the hello-world page and the health result.

Success criteria:
- One command (the start script) builds and runs the container.
- `/` serves placeholder HTML and the page successfully makes an API call.
- Stop script cleanly stops the container.

---

## Part 3: Add in Frontend (static build served by backend)

Goal: the real demo Kanban board is served at `/` from the container.

- [x] Set `output: 'export'` in `frontend/next.config.ts` (and any config needed for
      static export, e.g. image handling if applicable).
- [x] Confirm the app builds statically (`npm run build` produces `out/`).
- [x] Update the Docker build to build the frontend and copy `frontend/out/` into the
      static directory the backend serves (multi-stage: Node build stage, then Python
      runtime stage).
- [x] Remove the placeholder `index.html` (now replaced by the exported app); keep the
      static dir via `.gitkeep`, and update the backend static test accordingly.
- [x] Ensure client-side routing / assets resolve correctly under `/`.
- [x] Keep existing frontend unit tests passing; keep Playwright e2e for the demo board.

Tests:
- [x] Frontend unit tests pass: `npm run test:unit` (6 passed).
- [x] Frontend e2e pass against the dev server: `npm run test:e2e` (3 passed).
- [x] Integration: build the container (WSL2 Docker), open `/`, board HTML served
      (contains "Kanban Studio"), `_next` asset returns 200, health API still ok.

Success criteria:
- The container serves the real Kanban demo at `/` (not the placeholder).
- All existing frontend tests pass; the board is fully interactive in the container.

---

## Part 4: Fake user sign in

Goal: `/` requires login (`user` / `password`); logged-in users see the Kanban; logout
returns to the login screen. Session via HttpOnly cookie.

- [x] Backend auth routes:
  - [x] `POST /api/login` validates hardcoded credentials; on success sets an HttpOnly
        session cookie and returns the user; on failure returns 401.
  - [x] `POST /api/logout` clears the session cookie.
  - [x] `GET /api/me` returns the current user or 401 if not authenticated.
  - [x] Session mechanism: Starlette `SessionMiddleware` (signed HttpOnly cookie).
- [x] Frontend:
  - [x] Add a login screen (uses the color scheme; purple submit button).
  - [x] On load, call `GET /api/me`; show login if unauthenticated, board if authenticated.
  - [x] Add a logout control that calls `POST /api/logout` and returns to login.
  - [x] Show an error message on invalid credentials.
- [x] Handle unauthenticated API access consistently (401 -> show login).

Tests:
- [x] Backend unit tests: login success sets cookie; login failure returns 401; `me`
      reflects auth state; logout clears session (`uv run pytest`, 6 passed).
- [x] Frontend unit tests for the login form (submit, error state) (8 passed total).
- [x] E2E (against the container): login screen when signed out; wrong credentials show
      error; correct credentials show the board; reload keeps session; logout returns to
      login; demo board interactions after login (6 passed).

Success criteria:
- Cannot see the board without logging in; login/logout works; session persists across
  reloads via the cookie.

---

## Part 5: Database modeling

Goal: a documented SQLite schema storing the Kanban as JSON, signed off by the user.

- [x] Write `docs/DATABASE.md` describing:
  - [x] `users` table (id, username, unique constraint; password handling note for MVP).
  - [x] `boards` table (id, user_id FK, `data` JSON column holding `BoardData`, timestamps).
  - [x] Relationship: one board per user for the MVP; schema allows multiple.
  - [x] The JSON shape (mirrors frontend `BoardData`: columns + cards map).
  - [x] How the DB is created on first run if missing; seed strategy (seed `initialData`
        for a new user's board).
- [x] Note migration/versioning approach (kept minimal for MVP).
- [x] User reviews and signs off on the schema.

Tests / verification:
- [x] Schema doc reviewed against frontend `BoardData` for consistency.
- [x] User approves `docs/DATABASE.md`. DB access: stdlib `sqlite3` (no ORM).

Success criteria:
- `docs/DATABASE.md` exists, is consistent with the frontend model, and is approved.

---

## Part 6: Backend (Kanban API + persistence)

Goal: authenticated API to read and update a user's board, backed by SQLite, DB
auto-created if missing.

- [x] Add SQLite access (stdlib `sqlite3`; store `data` as JSON text).
- [x] On startup, create the DB and tables if they do not exist (lifespan `init_db`).
- [x] Implement routes (all require auth):
  - [x] `GET /api/board` returns the current user's board (creating a seeded board if the
        user has none).
  - [x] `PUT /api/board` replaces the current user's board with the posted `BoardData`
        (validated).
- [x] Validate the board payload shape (Pydantic models mirroring `BoardData`).
- [x] Seed a new user's board with the demo `initialData` equivalent.
- [x] Persist the DB across container recreation via a named Docker volume (`pm-data`).

Tests (backend unit/integration, thorough):
- [x] DB and tables auto-create when the file is absent.
- [x] `GET /api/board` returns a seeded board for a fresh user.
- [x] `PUT /api/board` persists changes; a subsequent `GET` returns them.
- [x] Unauthenticated requests to board routes return 401.
- [x] Invalid payloads return 422.
- [x] Persistence survives process restart (pytest: new client, same DB file; Docker:
      board edit survives container rm + recreate on the `pm-data` volume).

Success criteria:
- Board can be read and updated per user via the API, persisted in SQLite, with the DB
  created automatically. Backend tests pass.

---

## Part 7: Frontend + Backend integration

Goal: the frontend uses the backend API so the board is genuinely persistent per user.

- [x] On load (when authenticated), fetch the board from `GET /api/board` instead of
      using in-memory `initialData` (new `BoardView` container).
- [x] Persist changes via `PUT /api/board` (rename column, add/delete card, move card).
  - [x] Update strategy: save the whole board after each change, debounced (400ms) so a
        burst of edits collapses into one PUT.
  - [x] Handle loading and error states minimally.
- [x] Keep the `BoardData` type as the shared contract; `KanbanBoard` now takes
      `initialBoard` + `onBoardChange`, so it stays presentational/testable.
- [x] Ensure static-export build still works with these client-side fetches (same origin).

Tests (thorough):
- [x] Frontend unit tests for the data layer (getBoard/saveBoard) with mocked fetch
      (12 unit tests pass).
- [x] E2E against the running container: log in, add a card, reload, and confirm it
      persisted (loaded from the backend); session persists across reload; move/add/rename
      demo flows pass (7 e2e pass). Relogin persistence is covered by the backend
      `test_persists_across_restart` (a fresh session reads the persisted board).

Success criteria:
- The board is a real persistent Kanban: all edits survive reloads and re-logins. Unit and
  e2e tests pass.

---

## Part 8: AI connectivity

Goal: the backend can call OpenRouter and a simple sanity test works.

- [x] Add an OpenRouter client in the backend using `OPENROUTER_API_KEY` and model
      `openai/gpt-oss-120b` (`app/ai.py`, `chat()` via httpx).
- [x] Add a minimal internal test that asks the model "what is 2+2" and checks the
      response contains 4 (`test_chat_2plus2_live`).
- [x] Fail clearly if the API key is missing (`AIError`).

Tests:
- [x] Connectivity test: the "2+2" call returns a response containing 4 (skipped unless
      `OPENROUTER_API_KEY` is set; run live with the key from `.env` -> passed).
- [x] Unit test for the client with a mocked HTTP response, plus a missing-key test
      (`uv run pytest` -> 13 passed, 1 skipped without key).

Success criteria:
- The backend can successfully call the model and the 2+2 sanity check passes.

---

## Part 9: AI over the Kanban with Structured Outputs

Goal: the backend sends the board JSON plus the user's question and history to the model,
and receives a structured response (a reply plus an optional board update).

- [x] Define the structured output shape: `{ reply: string, board_update?: BoardData }`
      (`AiChatResult`). Uses JSON mode + Pydantic validation (see note below).
- [x] Add `POST /api/chat` (authenticated): accepts the user message and conversation
      history; loads the current board; calls the model; returns the structured result.
- [x] If `board_update` is present, validate it and persist it (reusing Part 6 logic).
- [x] Keep conversation history handling simple (client sends `history`).
- [x] Robustly handle model output that omits a board update (reply only) or proposes an
      invalid update (dropped, board left unchanged).

Note: the board's `cards` is a map keyed by arbitrary card ids, which strict OpenAI-style
`json_schema` Structured Outputs cannot express. We use JSON mode
(`response_format={"type": "json_object"}`) with server-side Pydantic validation, which is
robust for this shape.

Tests (thorough):
- [x] Unit tests with mocked model responses: reply-only; reply-with-valid-update
      (persisted); invalid update rejected without corrupting the stored board (18 passed).
- [x] Integration (live): "add a card 'Buy milk' to Backlog" yields a board update whose
      card count grows (`test_chat_about_board_live`, passed with key).
- [x] Auth required (401); invalid input returns 422.

Success criteria:
- `POST /api/chat` returns a structured reply and can update the persisted board correctly
  and safely. Tests pass.

---

## Part 10: AI chat sidebar UI

Goal: a polished chat sidebar; the AI can update the board and the UI refreshes
automatically when it does.

- [x] Add a sidebar chat widget matching the color scheme (blue/purple/yellow accents).
  - [x] Message list (user + assistant), input, send button, loading state.
  - [x] Sensible empty state and error handling.
- [x] Wire it to `POST /api/chat`, sending the message and history.
- [x] When the response includes a board update, refresh the board UI automatically
      (apply the returned board and remount the board via `key`).
- [x] Ensure layout works alongside the Kanban board without breaking drag-and-drop: the
      chat is a collapsible drawer (floating "Assistant" button), so the board stays
      full-width by default.

Tests (thorough):
- [x] Frontend unit tests: rendering messages, sending a message (mocked API), showing the
      reply, and triggering a board refresh when an update is returned (14 unit tests).
- [x] E2E against the container (live AI): open the sidebar, ask the AI to add a card,
      the assistant reply appears and the card shows in Backlog without a manual reload
      (8 e2e pass, incl. drag/add/persist/auth).

Success criteria:
- [x] A working, attractive AI chat drawer where the assistant can create/edit/move cards
      and the board refreshes automatically when it does. Unit and e2e tests pass.

---

## Overall done criteria

- Single Docker container: log in, see and edit a persistent Kanban board, and use an AI
  chat sidebar that can modify the board. Start/stop scripts for Mac, Windows, and Linux.
  All parts' tests pass.
