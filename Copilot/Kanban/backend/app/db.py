import os
import sqlite3
import uuid
from pathlib import Path

DEFAULT_DB_PATH = Path(__file__).parent.parent / "data" / "pm.db"

SCHEMA = """
PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS users (
    id         TEXT PRIMARY KEY,
    username   TEXT NOT NULL UNIQUE,
    created_at TEXT NOT NULL DEFAULT (datetime('now'))
);

CREATE TABLE IF NOT EXISTS boards (
    id      TEXT PRIMARY KEY,
    user_id TEXT NOT NULL UNIQUE REFERENCES users(id) ON DELETE CASCADE,
    title   TEXT NOT NULL DEFAULT 'My Board'
);

CREATE TABLE IF NOT EXISTS columns (
    id       TEXT PRIMARY KEY,
    board_id TEXT NOT NULL REFERENCES boards(id) ON DELETE CASCADE,
    title    TEXT NOT NULL,
    position INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS cards (
    id        TEXT PRIMARY KEY,
    column_id TEXT NOT NULL REFERENCES columns(id) ON DELETE CASCADE,
    title     TEXT NOT NULL,
    details   TEXT NOT NULL DEFAULT '',
    position  INTEGER NOT NULL
);
"""

SEED_COLUMNS = [
    ("col-backlog", "Backlog"),
    ("col-discovery", "Discovery"),
    ("col-progress", "In Progress"),
    ("col-review", "Review"),
    ("col-done", "Done"),
]

SEED_CARDS = [
    ("card-1", "col-backlog", "Align roadmap themes",
     "Draft quarterly themes with impact statements and metrics."),
    ("card-2", "col-backlog", "Gather customer signals",
     "Review support tags, sales notes, and churn feedback."),
    ("card-3", "col-discovery", "Prototype analytics view",
     "Sketch initial dashboard layout and key drill-downs."),
    ("card-4", "col-progress", "Refine status language",
     "Standardize column labels and tone across the board."),
    ("card-5", "col-progress", "Design card layout",
     "Add hierarchy and spacing for scanning dense lists."),
    ("card-6", "col-review", "QA micro-interactions",
     "Verify hover, focus, and loading states."),
    ("card-7", "col-done", "Ship marketing page",
     "Final copy approved and asset pack delivered."),
    ("card-8", "col-done", "Close onboarding sprint",
     "Document release notes and share internally."),
]


def db_path() -> Path:
    return Path(os.environ.get("PM_DB_PATH", str(DEFAULT_DB_PATH)))


def get_connection() -> sqlite3.Connection:
    path = db_path()
    path.parent.mkdir(parents=True, exist_ok=True)
    conn = sqlite3.connect(path)
    conn.row_factory = sqlite3.Row
    conn.execute("PRAGMA foreign_keys = ON")
    return conn


def new_id(prefix: str) -> str:
    return f"{prefix}-{uuid.uuid4().hex[:12]}"


def init_db() -> None:
    conn = get_connection()
    try:
        conn.executescript(SCHEMA)
        if conn.execute("SELECT COUNT(*) FROM users").fetchone()[0] == 0:
            _seed(conn)
        conn.commit()
    finally:
        conn.close()


def _seed(conn: sqlite3.Connection) -> None:
    conn.execute(
        "INSERT INTO users (id, username) VALUES (?, ?)", ("user-1", "user")
    )
    conn.execute(
        "INSERT INTO boards (id, user_id, title) VALUES (?, ?, ?)",
        ("board-1", "user-1", "My Board"),
    )
    for position, (column_id, title) in enumerate(SEED_COLUMNS):
        conn.execute(
            "INSERT INTO columns (id, board_id, title, position) VALUES (?, ?, ?, ?)",
            (column_id, "board-1", title, position),
        )
    counters: dict[str, int] = {}
    for card_id, column_id, title, details in SEED_CARDS:
        position = counters.get(column_id, 0)
        counters[column_id] = position + 1
        conn.execute(
            "INSERT INTO cards (id, column_id, title, details, position) "
            "VALUES (?, ?, ?, ?, ?)",
            (card_id, column_id, title, details, position),
        )
