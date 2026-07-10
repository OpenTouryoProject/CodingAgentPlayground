# Database

SQLite database for the Project Management MVP. Created automatically on first
run if it does not exist (implemented in Part 6). Stored as a local file
(`backend/data/pm.db`).

## Goals

- Support multiple users (future); the MVP signs in a single hardcoded user.
- One board per user for the MVP (enforced by a unique constraint).
- Renameable columns and ordered cards.
- Map cleanly to the board JSON the frontend already uses and the AI will read.

## Schema

```sql
PRAGMA foreign_keys = ON;

CREATE TABLE users (
    id         TEXT PRIMARY KEY,
    username   TEXT NOT NULL UNIQUE,
    created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE boards (
    id      TEXT PRIMARY KEY,
    user_id TEXT NOT NULL UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    title   TEXT NOT NULL DEFAULT 'My Board'
);

CREATE TABLE columns (
    id       TEXT PRIMARY KEY,
    board_id TEXT NOT NULL REFERENCES boards(id) ON DELETE CASCADE,
    title    TEXT NOT NULL,
    position INTEGER NOT NULL
);

CREATE TABLE cards (
    id        TEXT PRIMARY KEY,
    column_id TEXT NOT NULL REFERENCES columns(id) ON DELETE CASCADE,
    title     TEXT NOT NULL,
    details   TEXT NOT NULL DEFAULT '',
    position  INTEGER NOT NULL
);
```

### Notes

- IDs are text (e.g. `col-backlog`, `card-1`), matching the frontend id style.
- `boards.user_id UNIQUE` enforces one board per user for the MVP.
- Ordering is explicit via `position` (0-based) within the parent: columns within
  a board, cards within a column.
- `ON DELETE CASCADE` keeps cleanup simple (delete a column, its cards go too).

## Board JSON shape

The API returns and accepts the board in the exact shape the frontend already
uses (`BoardData`), so the AI receives the same structure. Columns are ordered;
each column lists its `cardIds` in order; cards are keyed by id.

```json
{
  "columns": [
    { "id": "col-backlog", "title": "Backlog", "cardIds": ["card-1"] }
  ],
  "cards": {
    "card-1": { "id": "card-1", "title": "...", "details": "..." }
  }
}
```

A full example is in `docs/example-board.json`.

### Mapping DB to JSON

- `columns` array = `columns` rows for the board, ordered by `position`.
- Each column's `cardIds` = `cards` rows for that column, ordered by `position`.
- `cards` object = every card on the board, keyed by `id`.

## Seeding

On first run the DB is created and seeded with:
- one user `user` (matches the hardcoded login),
- one board for that user,
- the demo columns and cards from `docs/example-board.json`.
