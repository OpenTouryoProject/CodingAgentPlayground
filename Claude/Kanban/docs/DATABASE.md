# Database

SQLite, created automatically on first run if the file does not exist. The Kanban board
is stored as a single JSON blob per user, matching the frontend `BoardData` shape.

- File: `backend/app/data/app.db` (git-ignored).
- Access: Python stdlib `sqlite3` (no ORM; keep it simple).
- One board per user for the MVP; the schema allows multiple users and, later, multiple
  boards per user.

## Tables

### users

| Column     | Type    | Notes                                  |
|------------|---------|----------------------------------------|
| id         | INTEGER | PRIMARY KEY AUTOINCREMENT               |
| username   | TEXT    | NOT NULL, UNIQUE                        |
| created_at | TEXT    | NOT NULL, ISO-8601 UTC                  |

For the MVP there is a single seeded user `user`. Passwords are not stored: the login is
hardcoded (`user` / `password`) in the backend. A `password_hash` column can be added
later without changing the board storage.

### boards

| Column     | Type    | Notes                                       |
|------------|---------|---------------------------------------------|
| id         | INTEGER | PRIMARY KEY AUTOINCREMENT                    |
| user_id    | INTEGER | NOT NULL, REFERENCES users(id)              |
| data       | TEXT    | NOT NULL, JSON-encoded `BoardData`          |
| updated_at | TEXT    | NOT NULL, ISO-8601 UTC                       |

The MVP reads/writes the single board belonging to the current user. A future multi-board
setup would add a `name`/`position` and drop the one-per-user assumption; no change to the
`data` format is needed.

## Board JSON (`data` column)

The `data` column holds exactly the frontend `BoardData` (see
`frontend/src/lib/kanban.ts`). Cards live in a map keyed by id; each column lists its card
ids in order.

```json
{
  "columns": [
    { "id": "col-backlog", "title": "Backlog", "cardIds": ["card-1", "card-2"] },
    { "id": "col-discovery", "title": "Discovery", "cardIds": ["card-3"] },
    { "id": "col-progress", "title": "In Progress", "cardIds": ["card-4", "card-5"] },
    { "id": "col-review", "title": "Review", "cardIds": ["card-6"] },
    { "id": "col-done", "title": "Done", "cardIds": ["card-7", "card-8"] }
  ],
  "cards": {
    "card-1": { "id": "card-1", "title": "Align roadmap themes", "details": "..." },
    "card-3": { "id": "card-3", "title": "Prototype analytics view", "details": "..." }
  }
}
```

Types (mirrors the frontend):

- `Card`: `{ id: string, title: string, details: string }`
- `Column`: `{ id: string, title: string, cardIds: string[] }`
- `BoardData`: `{ columns: Column[], cards: Record<string, Card> }`

## Creation and seeding

- On startup the backend connects to the SQLite file, creating it and running
  `CREATE TABLE IF NOT EXISTS` for both tables.
- The MVP user `user` is inserted if absent.
- When a user first requests their board and has none, a board seeded with the demo
  `initialData` (the five columns and eight sample cards) is created and returned.

## Consistency and integrity

- The backend validates the board payload against Pydantic models mirroring `BoardData`
  before writing, so the stored JSON is always well-formed (Part 6).
- `PUT /api/board` replaces the whole `data` blob (the board is small; no partial updates).
- `updated_at` is set on every write.

## Migrations

No migration framework for the MVP. Schema changes are additive via
`CREATE TABLE IF NOT EXISTS` / `ALTER TABLE` at startup if ever needed. Because the board
lives in a single JSON column, board-shape changes are handled in application code, not
DDL.
