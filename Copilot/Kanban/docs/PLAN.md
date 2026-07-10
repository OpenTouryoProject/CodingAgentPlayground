# Project Management MVP - Implementation Plan

A single-board Kanban app: Next.js frontend, FastAPI backend (serves the static
frontend and the API), SQLite storage, and an OpenRouter AI chat that can edit
the board. Runs locally in one Docker container.

This plan expands the 10 parts, each with a goal, a substep checklist, tests,
and success criteria. Parts are done in order; each part must pass its success
criteria before the next begins.

## Conventions

- Keep it simple. No over-engineering, no speculative features.
- No emojis anywhere.
- Colors: Yellow `#ecad0a`, Blue `#209dd7`, Purple `#753991`, Navy `#032147`,
  Gray `#888888`.
- Backend: Python FastAPI, `uv` package manager, SQLite (auto-created).
- AI: OpenRouter, model `openai/gpt-oss-120b`, key from root `.env`
  (`OPENROUTER_API_KEY`).
- Frontend: Next.js (currently 16) / React 19 / Tailwind v4 / dnd-kit.

## Open decision points (resolved at the noted part)

- [ ] Frontend serving strategy: Next.js static export (`output: 'export'`) vs
  prebuilt assets copied into the image - decide at Part 3.
- [ ] Auth mechanism for hardcoded `user` / `password`: server-set session
  cookie via FastAPI - decide at Part 4.
- [ ] Board persistence model and JSON shape - decide and sign off at Part 5.
- [ ] AI Structured Outputs schema (reply text + optional board update) - decide
  at Part 9.

---

## Part 1: Plan

Goal: Produce a detailed, approved plan and document the existing frontend.

- [ ] Enrich this `docs/PLAN.md` with per-part checklists, tests, and success
  criteria (this document).
- [ ] Fold the missing card-edit feature (edit title/details) into the frontend
  parts.
- [ ] Create `frontend/AGENTS.md` describing the existing frontend code.
- [ ] Get user review and approval before Part 2.

Tests: none (documentation only). Verify no emojis and internal consistency.

Success criteria:
- This document lists all 10 parts with substeps, tests, and success criteria.
- `frontend/AGENTS.md` exists and accurately describes the current frontend.
- User has approved the plan.

---

## Part 2: Scaffolding

Goal: Stand up Docker + FastAPI serving a static "hello world" page and one API
endpoint, plus start/stop scripts.

- [x] Create `backend/` FastAPI app managed by `uv` (`pyproject.toml`).
- [x] Add a health/API route (e.g. `GET /api/health` returns JSON).
- [x] Serve a static `index.html` "hello world" at `/`.
- [x] Write a `Dockerfile` using `uv` to install and run the backend.
- [x] Write start/stop scripts in `scripts/` for Mac, PC (PowerShell), Linux.
- [x] Update `backend/AGENTS.md` with the backend description.

Tests:
- [x] Backend unit test for `GET /api/health` (pytest + FastAPI TestClient).
- [x] Manual: `docker` build + run, `GET /` returns the page, `GET /api/health`
  returns JSON.

Success criteria:
- Container builds and runs locally via the start script; stop script stops it.
- `/` serves static HTML and the API endpoint responds with expected JSON.

---

## Part 3: Add in Frontend

Goal: Build the existing Kanban frontend statically and serve it from FastAPI at
`/`; add the card-edit feature; comprehensive tests.

- [x] Decide and apply the serving strategy (static export vs copied build).
- [x] Configure the frontend build to produce static assets.
- [x] Wire FastAPI to serve the built frontend at `/` (with SPA/asset routing).
- [x] Add card editing (edit a card's title and details) to the frontend UI and
  the `kanban` state logic.
- [x] Integrate the frontend build step into the Docker image.

Tests:
- [x] Unit: extend `kanban.test.ts` for the edit operation.
- [x] Unit: component test for the card edit UI (open, change, save, cancel).
- [x] E2E (playwright): load board at `/`, add, move, and edit a card.
- [x] Integration: served build renders the board at `/` from the backend.

Success criteria:
- Visiting `/` on the running container shows the demo Kanban board.
- Cards can be added, moved, deleted, and edited in the UI.
- All unit and e2e tests pass.

---

## Part 4: Fake user sign in

Goal: Require login with `user` / `password` before the board; support logout.

- [x] Add backend auth routes (login, logout) validating hardcoded credentials.
- [x] Set a session cookie on successful login; clear it on logout.
- [x] Protect the board/app so unauthenticated users are sent to a login screen.
- [x] Add a login page and a logout control to the frontend.

Tests:
- [x] Backend unit: login success, login failure, logout, protected route
  without/with session.
- [x] Component: login form validation and submit.
- [x] E2E: unauthenticated `/` shows login; valid login shows board; logout
  returns to login.

Success criteria:
- Wrong credentials are rejected; correct ones grant access.
- Board is inaccessible without a valid session; logout works.
- All tests pass.

---

## Part 5: Database modeling

Goal: Propose the Kanban DB schema, capture a JSON representation, and document
it for sign-off.

- [x] Design tables supporting multiple users, one board per user, columns, and
  cards (with ordering).
- [x] Define the board JSON shape used by the API and AI.
- [x] Save an example board as JSON and document the schema in `docs/`.
- [x] Get user sign-off on the schema before Part 6.

Tests: none (design artifact). Validate the example JSON parses and matches the
documented shape.

Success criteria:
- `docs/` contains the schema description and an example board JSON.
- Schema covers users, single board per user, renameable columns, ordered cards.
- User has signed off.

---

## Part 6: Backend

Goal: API routes to read and modify a user's Kanban, backed by SQLite that is
created if missing; thorough unit tests.

- [x] Create the SQLite database and tables on startup if absent.
- [x] Implement read endpoint returning the user's board as JSON.
- [x] Implement write endpoints (rename column, create/edit/move/delete card).
- [x] Scope all reads/writes to the authenticated user.

Tests:
- [x] Backend unit: DB auto-creation on first run.
- [x] Backend unit: get board, rename column, create/edit/move/delete card.
- [x] Backend unit: auth scoping (a user only sees/edits their own board).

Success criteria:
- Fresh run creates the DB and a default board.
- Every board mutation persists and reloads correctly.
- All backend unit tests pass.

---

## Part 7: Frontend + Backend

Goal: Frontend uses the backend API so the board is persistent; thorough tests.

- [x] Replace in-memory board state with data loaded from the API.
- [x] Send all mutations (rename, add, edit, move, delete) to the API and reflect
  results.
- [x] Handle loading and error states simply.

Tests:
- [x] Component/integration: board loads from a mocked API and renders.
- [x] Integration: each mutation calls the correct endpoint and updates the UI.
- [x] E2E: change the board, reload, and confirm changes persisted.

Success criteria:
- Board state survives reloads and container restarts (DB-backed).
- All mutations persist through the API.
- All tests pass.

---

## Part 8: AI connectivity

Goal: Backend can call OpenRouter; verify with a simple "2+2" test.

- [x] Add an OpenRouter client using `OPENROUTER_API_KEY` and model
  `openai/gpt-oss-120b`.
- [x] Add a minimal internal call/endpoint to send a prompt and return the reply.
- [x] Verify a "2+2" prompt returns "4".

Tests:
- [x] Backend unit: client builds the request correctly (mocked HTTP).
- [x] Connectivity check: live "2+2" call returns "4" (run manually / gated).

Success criteria:
- A live "2+2" call returns the correct answer.
- Missing/invalid key fails clearly.

---

## Part 9: AI board reasoning with Structured Outputs

Goal: Every AI call includes the board JSON plus the user's question and history;
the AI replies with Structured Outputs containing the user reply and an optional
board update.

- [x] Define the Structured Outputs schema (reply text + optional board update).
- [x] Build the request: system context + board JSON + conversation history +
  user question.
- [x] Parse the structured response; apply any board update via the Part 6 layer.
- [x] Return the reply and updated board to the caller.

Tests:
- [x] Backend unit: prompt assembly includes board JSON and history.
- [x] Backend unit: structured response parsing (reply only; reply + update).
- [x] Backend unit: a board update from the AI persists correctly.
- [x] Behavioral: "move card X to Done" yields a valid board update.

Success criteria:
- AI responses conform to the schema.
- AI-produced updates validate and persist.
- All tests pass.

---

## Part 10: AI chat sidebar

Goal: A polished sidebar chat; the AI can update the board via its Structured
Outputs, and the UI refreshes automatically when it does.

- [x] Add a sidebar chat widget (message list + input) styled to the color
  scheme.
- [x] Send question + history to the AI endpoint and render replies.
- [x] When the response includes a board update, refresh the board UI.
- [x] Keep the layout responsive alongside the board.

Tests:
- [x] Component: chat renders messages and sends input.
- [x] Integration: a response with a board update refreshes the board.
- [x] E2E: ask the AI to change the board; the board updates without manual
  reload.

Success criteria:
- Users chat with the AI from the sidebar.
- AI-driven board changes appear automatically.
- All tests pass and the full app runs in the Docker container.
