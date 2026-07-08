# Frontend

The existing frontend is a pure client-side Kanban demo ("Kanban Studio"). It has no
backend calls and holds all state in memory, resetting on reload. Later plan parts wire
it to the FastAPI backend, add auth, and add an AI chat sidebar.

## Stack

- Next.js 16 (App Router) with React 19
- TypeScript
- Tailwind CSS v4 (via `@tailwindcss/postcss`)
- `@dnd-kit/core`, `@dnd-kit/sortable`, `@dnd-kit/utilities` for drag and drop
- `clsx` for conditional classes
- Fonts: `next/font/google` (Space Grotesk for display, Manrope for body)
- Testing: Vitest + Testing Library (unit), Playwright (e2e)

## Layout

```
frontend/
  src/
    app/
      layout.tsx        Root layout, fonts, metadata
      page.tsx          Home route, renders <AppShell />
      globals.css       Tailwind import + CSS variables (color scheme)
    components/
      AppShell.tsx           Client component; auth gating (login vs board) + logout
      LoginForm.tsx          Sign-in form (calls onLogin, shows errors)
      LoginForm.test.tsx     Vitest component tests for the login form
      BoardView.tsx          Loads the board + saves changes; hosts the board and chat
      ChatSidebar.tsx        Collapsible AI chat drawer; calls /api/chat, applies updates
      ChatSidebar.test.tsx   Vitest tests for the chat sidebar (mocked api)
      KanbanBoard.tsx        Board UI; takes initialBoard + onBoardChange, owns edit state
      KanbanColumn.tsx       A single droppable column with sortable cards
      KanbanCard.tsx         A draggable/sortable card with a Remove button
      KanbanCardPreview.tsx  Static card used inside the drag overlay
      NewCardForm.tsx        Collapsible add-card form
      KanbanBoard.test.tsx   Vitest component tests
    lib/
      kanban.ts         Types, initial seed data, and pure board helpers
      kanban.test.ts    Vitest unit tests for kanban.ts
      api.ts            Backend fetch helpers (getMe, login, logout, getBoard, saveBoard, sendChat)
      api.test.ts       Vitest unit tests for the board API helpers (mocked fetch)
    test/
      setup.ts          Vitest setup (jest-dom matchers)
      vitest.d.ts       Type augmentation for jest-dom
  tests/
    helpers.ts          Shared e2e helpers (login)
    auth.spec.ts        Playwright e2e for login/logout/session
    kanban.spec.ts      Playwright e2e (load, add card, persist, drag between columns)
    chat.spec.ts        Playwright e2e (live): AI adds a card, board auto-refreshes
  next.config.ts        Next config (output: 'export' for static export)
  vitest.config.ts      Vitest config (jsdom, @ alias, coverage)
  playwright.config.ts  Playwright config (targets the running full app)
  package.json          Scripts and dependencies
```

## Auth

The board is gated behind a sign-in. `AppShell` checks `GET /api/me` on load and shows
`LoginForm` when signed out or the board (plus a logout control) when signed in. Login and
logout go through `src/lib/api.ts`. The session is a backend HttpOnly cookie, so the
frontend never stores credentials. Hardcoded MVP credentials are `user` / `password`.

## AI chat

`ChatSidebar` is a collapsible drawer (a floating "Assistant" button opens it) so the board
stays full-width by default. It sends the message plus conversation history to
`POST /api/chat`. When the response includes a `board_update`, it calls `onBoardUpdate`;
`BoardView` swaps in the new board and bumps a `key` to remount `KanbanBoard`, so the board
refreshes automatically (the update is already persisted server-side, so no extra save).

## Data model (`src/lib/kanban.ts`)

- `Card`: `{ id, title, details }`
- `Column`: `{ id, title, cardIds: string[] }` — column owns the ordering of card ids
- `BoardData`: `{ columns: Column[]; cards: Record<string, Card> }` — cards stored in a
  lookup map keyed by id, referenced from columns by id

`initialData` seeds five columns (Backlog, Discovery, In Progress, Review, Done) with
eight sample cards.

Pure helpers:

- `moveCard(columns, activeId, overId)`: returns new `columns` with a card moved. Handles
  reordering within a column and moving across columns, and dropping onto an empty column
  (when `overId` is a column id). Returns the input unchanged when the move is a no-op.
- `createId(prefix)`: generates a short unique id (`prefix-<random><time>`).

## State and behavior

`KanbanBoard` is the only stateful component (`"use client"`). It holds `board` and the
currently dragged `activeCardId`, and passes handlers down:

- Rename column: controlled `input` per column bound to `onRename`
- Add card: `NewCardForm` -> `onAddCard` (title required; details default to a placeholder)
- Delete card: Remove button -> `onDeleteCard`
- Drag and drop: `DndContext` with a `PointerSensor` (6px activation distance) and
  `closestCorners` collision detection; `DragOverlay` renders `KanbanCardPreview`

All state is in memory only. There is no persistence yet.

## Styling

Colors are CSS variables in `globals.css`, matching the project color scheme:

- `--accent-yellow: #ecad0a`
- `--primary-blue: #209dd7`
- `--secondary-purple: #753991`
- `--navy-dark: #032147`
- `--gray-text: #888888`

Plus surface/stroke/shadow tokens. Tailwind utility classes reference these via
`var(--...)`. `data-testid` attributes (`column-<id>`, `card-<id>`) support tests.

## Commands

```bash
npm install
npm run dev            # dev server
npm run build          # production build
npm run lint           # eslint
npm run test:unit      # vitest (unit/component)
npm run test:e2e       # playwright, against the running app (see below)
npm run test:all       # unit then e2e
```

E2E runs against the full app served by the backend at the same origin (login needs the
backend). Start the container (`scripts/start.*`, port 8000) first, then run
`npm run test:e2e`. Override the target with `PLAYWRIGHT_BASE_URL`.

## Notes for later plan parts

- The app is fully client-rendered, so a Next.js static export (`output: 'export'`) is
  viable for serving via FastAPI `StaticFiles`. No SSR/server actions are used.
- `BoardData` is the shared contract with the backend (stored as a JSON blob per board).
  Keep the frontend `BoardData` type and the backend Pydantic models in sync.
- The board is loaded from `GET /api/board` and saved via `PUT /api/board` (debounced) by
  `BoardView`. `initialData` is only the fallback default inside `KanbanBoard` and the
  backend seed source; the live board comes from the backend.
