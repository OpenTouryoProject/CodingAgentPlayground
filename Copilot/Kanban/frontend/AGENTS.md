# Frontend

Next.js single-board Kanban app. The board is loaded from and persisted through
the FastAPI backend (`/api`); auth gates access. Remaining work in
`docs/PLAN.md` adds the AI chat sidebar (Part 10).

## Stack

- Next.js 16 (App Router) + React 19, TypeScript.
- Tailwind CSS v4 (`@tailwindcss/postcss`), CSS variables in `globals.css`.
- Drag and drop: `@dnd-kit/core`, `@dnd-kit/sortable`, `@dnd-kit/utilities`.
- `clsx` for conditional classes.
- Fonts: Space Grotesk (display) and Manrope (body) via `next/font`.
- Tests: Vitest + Testing Library (unit), Playwright (e2e).

## Scripts (package.json)

- `dev` - Next dev server.
- `build` / `start` - production build / serve.
- `lint` - ESLint (`eslint-config-next`).
- `test` / `test:unit` - Vitest run; `test:unit:watch` - watch mode.
- `test:e2e` - Playwright; `test:all` - unit then e2e.

## Layout

- `src/app/layout.tsx` - root layout, fonts, metadata.
- `src/app/page.tsx` - renders `<AppShell />`.
- `src/app/globals.css` - Tailwind + color-scheme CSS variables.
- `src/lib/kanban.ts` - data model and pure board logic.
- `src/lib/api.ts` - typed fetch wrappers for the board API.
- `src/components/` - Kanban UI components.
- `src/test/` - Vitest setup (`setup.ts`, `vitest.d.ts`).
- `tests/kanban.spec.ts` - Playwright e2e.

## Data model (`src/lib/kanban.ts`)

- `Card` = `{ id, title, details }`.
- `Column` = `{ id, title, cardIds: string[] }`.
- `BoardData` = `{ columns: Column[]; cards: Record<string, Card> }`.
- `initialData` - seed board (5 columns, 8 cards) used by tests as a fixture; the
  live board comes from the backend.
- `moveCard(columns, activeId, overId)` - pure reducer for reordering within and
  moving between columns (handles column-target and card-target drops).
- `updateCard(cards, cardId, title, details)` - pure update of a card's title and
  details.
- `createId(prefix)` - id generator.

## API client (`src/lib/api.ts`)

Thin `fetch` wrappers, each returning the updated `BoardData`:
`fetchBoard`, `renameColumnApi`, `createCardApi`, `updateCardApi`, `moveCardApi`,
`deleteCardApi`. The backend returns the full board after every mutation.
`chatApi(question, history)` posts to `/api/ai/chat` and returns `{reply, board}`.

## Components

- `KanbanBoard.tsx` (`"use client"`) - loads the board from `GET /api/board` on
  mount (loading and error states), holds it in `useState`, and sends every
  mutation to the backend via `src/lib/api.ts`, replacing state with the returned
  board. dnd-kit `DndContext` with `PointerSensor` and `closestCorners`; drag end
  optimistically reorders then calls the move endpoint. Renders the header and a
  5-column grid plus a `DragOverlay`.
- `KanbanColumn.tsx` - droppable column; title input holds local state and commits
  the rename to the backend on blur, a `SortableContext` of cards, an empty-state,
  and `NewCardForm`.
- `KanbanCard.tsx` - sortable/draggable card; shows title + details with Edit and
  Remove buttons. Edit mode swaps in title/details inputs with Save and Cancel.
- `KanbanCardPreview.tsx` - static card visual used in the drag overlay.
- `NewCardForm.tsx` - collapsible add-card form (title required, details
  optional).
- `AppShell.tsx` (`"use client"`) - auth gate. Calls `GET /api/me` on mount;
  shows `LoginForm` when unauthenticated, otherwise `KanbanBoard`. Provides
  logout via `POST /api/logout`.
- `LoginForm.tsx` - username/password form posting to `/api/login`; shows an
  error on failure.
- `ChatSidebar.tsx` (`"use client"`) - AI assistant panel: message list + input,
  posts `{question, history}` to `chatApi`, renders replies, and calls
  `onBoardUpdate` with the returned board so the board refreshes automatically.
  Rendered beside the board in `KanbanBoard` (stacked below on narrow screens,
  a right-hand column at `xl`).

## State flow

`KanbanBoard` loads the board from `GET /api/board` and holds it in React state.
Every mutation (rename column, add/edit/move/delete card) is sent to the backend,
which returns the full updated board that replaces local state. State therefore
persists across reloads and restarts (SQLite-backed).

## Color scheme (CSS variables in globals.css)

Yellow `#ecad0a`, Blue `#209dd7`, Purple `#753991`, Navy `#032147`, Gray
`#888888`, referenced as `--accent-yellow`, `--primary-blue`,
`--secondary-purple`, `--navy-dark`, `--gray-text`.

## Testing

- Vitest config (`vitest.config.ts`): jsdom, globals, setup file, `@` alias to
  `src`, includes `src/**/*.{test,spec}.{ts,tsx}`, excludes `tests/`.
- Unit tests: `src/lib/kanban.test.ts`, `src/components/KanbanBoard.test.tsx`,
  `src/components/ChatSidebar.test.tsx`, `src/components/LoginForm.test.tsx`.
- Playwright (`playwright.config.ts`): baseURL `http://127.0.0.1:3000`, starts
  `next dev`; `tests/kanban.spec.ts` covers load, add, move, and edit card (each
  verified to persist across a reload), an AI-chat board update, plus
  login/logout. The board, AI chat, and auth API are stubbed via Playwright route
  mocking backed by in-memory state.

## Build and serving

- `next.config.ts` sets `output: "export"`, so `npm run build` emits a static
  site to `frontend/out/`.
- The FastAPI backend serves that build at `/` (copied into
  `backend/app/static/` during the Docker build).

## Known gaps (addressed by docs/PLAN.md)

All roadmap parts are implemented.
