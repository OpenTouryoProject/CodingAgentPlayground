import os
import sqlite3
from contextlib import contextmanager
from datetime import datetime, timezone
from pathlib import Path

from .board import BoardData, initial_board

DEFAULT_DB_PATH = Path(__file__).parent / "data" / "app.db"


def _db_path() -> Path:
    return Path(os.environ.get("APP_DB_PATH", str(DEFAULT_DB_PATH)))


def _now() -> str:
    return datetime.now(timezone.utc).isoformat()


@contextmanager
def _connect():
    path = _db_path()
    path.parent.mkdir(parents=True, exist_ok=True)
    conn = sqlite3.connect(path)
    conn.row_factory = sqlite3.Row
    conn.execute("PRAGMA foreign_keys = ON")
    try:
        yield conn
        conn.commit()
    finally:
        conn.close()


def init_db() -> None:
    with _connect() as conn:
        conn.execute(
            """
            CREATE TABLE IF NOT EXISTS users (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                username TEXT NOT NULL UNIQUE,
                created_at TEXT NOT NULL
            )
            """
        )
        conn.execute(
            """
            CREATE TABLE IF NOT EXISTS boards (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_id INTEGER NOT NULL REFERENCES users(id),
                data TEXT NOT NULL,
                updated_at TEXT NOT NULL
            )
            """
        )


def _user_id(conn: sqlite3.Connection, username: str) -> int:
    row = conn.execute(
        "SELECT id FROM users WHERE username = ?", (username,)
    ).fetchone()
    if row:
        return row["id"]
    cur = conn.execute(
        "INSERT INTO users (username, created_at) VALUES (?, ?)",
        (username, _now()),
    )
    return cur.lastrowid


def get_board(username: str) -> BoardData:
    with _connect() as conn:
        user_id = _user_id(conn, username)
        row = conn.execute(
            "SELECT data FROM boards WHERE user_id = ? ORDER BY id LIMIT 1",
            (user_id,),
        ).fetchone()
        if row:
            return BoardData.model_validate_json(row["data"])

        board = initial_board()
        conn.execute(
            "INSERT INTO boards (user_id, data, updated_at) VALUES (?, ?, ?)",
            (user_id, board.model_dump_json(), _now()),
        )
        return board


def save_board(username: str, board: BoardData) -> None:
    with _connect() as conn:
        user_id = _user_id(conn, username)
        data = board.model_dump_json()
        row = conn.execute(
            "SELECT id FROM boards WHERE user_id = ? ORDER BY id LIMIT 1",
            (user_id,),
        ).fetchone()
        if row:
            conn.execute(
                "UPDATE boards SET data = ?, updated_at = ? WHERE id = ?",
                (data, _now(), row["id"]),
            )
        else:
            conn.execute(
                "INSERT INTO boards (user_id, data, updated_at) VALUES (?, ?, ?)",
                (user_id, data, _now()),
            )
