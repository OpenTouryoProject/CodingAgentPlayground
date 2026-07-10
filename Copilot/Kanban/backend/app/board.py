import sqlite3

from app.db import new_id


class NotFound(Exception):
    pass


def get_board(conn: sqlite3.Connection, board_id: str) -> dict:
    columns = []
    cards: dict[str, dict] = {}
    column_rows = conn.execute(
        "SELECT id, title FROM columns WHERE board_id = ? ORDER BY position",
        (board_id,),
    ).fetchall()
    for column in column_rows:
        card_rows = conn.execute(
            "SELECT id, title, details FROM cards WHERE column_id = ? ORDER BY position",
            (column["id"],),
        ).fetchall()
        card_ids = []
        for card in card_rows:
            cards[card["id"]] = {
                "id": card["id"],
                "title": card["title"],
                "details": card["details"],
            }
            card_ids.append(card["id"])
        columns.append(
            {"id": column["id"], "title": column["title"], "cardIds": card_ids}
        )
    return {"columns": columns, "cards": cards}


def rename_column(
    conn: sqlite3.Connection, board_id: str, column_id: str, title: str
) -> None:
    cursor = conn.execute(
        "UPDATE columns SET title = ? WHERE id = ? AND board_id = ?",
        (title, column_id, board_id),
    )
    if cursor.rowcount == 0:
        raise NotFound
    conn.commit()


def create_card(
    conn: sqlite3.Connection,
    board_id: str,
    column_id: str,
    title: str,
    details: str,
) -> str:
    column = conn.execute(
        "SELECT id FROM columns WHERE id = ? AND board_id = ?",
        (column_id, board_id),
    ).fetchone()
    if column is None:
        raise NotFound
    position = conn.execute(
        "SELECT COALESCE(MAX(position) + 1, 0) FROM cards WHERE column_id = ?",
        (column_id,),
    ).fetchone()[0]
    card_id = new_id("card")
    conn.execute(
        "INSERT INTO cards (id, column_id, title, details, position) "
        "VALUES (?, ?, ?, ?, ?)",
        (card_id, column_id, title, details, position),
    )
    conn.commit()
    return card_id


def update_card(
    conn: sqlite3.Connection,
    board_id: str,
    card_id: str,
    title: str,
    details: str,
) -> None:
    cursor = conn.execute(
        "UPDATE cards SET title = ?, details = ? WHERE id = ? AND column_id IN "
        "(SELECT id FROM columns WHERE board_id = ?)",
        (title, details, card_id, board_id),
    )
    if cursor.rowcount == 0:
        raise NotFound
    conn.commit()


def delete_card(conn: sqlite3.Connection, board_id: str, card_id: str) -> None:
    column_id = _card_column(conn, board_id, card_id)
    conn.execute("DELETE FROM cards WHERE id = ?", (card_id,))
    _reindex(conn, column_id)
    conn.commit()


def move_card(
    conn: sqlite3.Connection,
    board_id: str,
    card_id: str,
    target_column_id: str,
    position: int,
) -> None:
    source_column_id = _card_column(conn, board_id, card_id)
    target = conn.execute(
        "SELECT id FROM columns WHERE id = ? AND board_id = ?",
        (target_column_id, board_id),
    ).fetchone()
    if target is None:
        raise NotFound

    conn.execute(
        "UPDATE cards SET column_id = ? WHERE id = ?",
        (target_column_id, card_id),
    )
    ordered = [
        row["id"]
        for row in conn.execute(
            "SELECT id FROM cards WHERE column_id = ? AND id != ? ORDER BY position",
            (target_column_id, card_id),
        ).fetchall()
    ]
    position = max(0, min(position, len(ordered)))
    ordered.insert(position, card_id)
    for index, moved_id in enumerate(ordered):
        conn.execute(
            "UPDATE cards SET position = ? WHERE id = ?", (index, moved_id)
        )
    if source_column_id != target_column_id:
        _reindex(conn, source_column_id)
    conn.commit()


def apply_actions(
    conn: sqlite3.Connection, board_id: str, actions: list[dict]
) -> None:
    for action in actions:
        kind = action["type"]
        if kind == "create_card":
            create_card(
                conn,
                board_id,
                action["columnId"],
                action["title"] or "",
                action.get("details") or "",
            )
        elif kind == "update_card":
            update_card(
                conn,
                board_id,
                action["cardId"],
                action["title"] or "",
                action.get("details") or "",
            )
        elif kind == "move_card":
            move_card(
                conn,
                board_id,
                action["cardId"],
                action["columnId"],
                action["position"] or 0,
            )
        elif kind == "delete_card":
            delete_card(conn, board_id, action["cardId"])
        elif kind == "rename_column":
            rename_column(conn, board_id, action["columnId"], action["title"] or "")


def _card_column(conn: sqlite3.Connection, board_id: str, card_id: str) -> str:
    row = conn.execute(
        "SELECT cards.column_id AS column_id FROM cards "
        "JOIN columns ON cards.column_id = columns.id "
        "WHERE cards.id = ? AND columns.board_id = ?",
        (card_id, board_id),
    ).fetchone()
    if row is None:
        raise NotFound
    return row["column_id"]


def _reindex(conn: sqlite3.Connection, column_id: str) -> None:
    ordered = conn.execute(
        "SELECT id FROM cards WHERE column_id = ? ORDER BY position",
        (column_id,),
    ).fetchall()
    for index, row in enumerate(ordered):
        conn.execute(
            "UPDATE cards SET position = ? WHERE id = ?", (index, row["id"])
        )
