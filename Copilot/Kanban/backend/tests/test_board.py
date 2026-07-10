import pytest
from fastapi.testclient import TestClient


@pytest.fixture
def client(tmp_path, monkeypatch):
    monkeypatch.setenv("PM_DB_PATH", str(tmp_path / "test.db"))
    from app.db import init_db

    init_db()
    from app.main import app

    test_client = TestClient(app)
    test_client.post(
        "/api/login", json={"username": "user", "password": "password"}
    )
    return test_client


def test_board_created_and_seeded(client):
    response = client.get("/api/board")
    assert response.status_code == 200
    board = response.json()
    assert len(board["columns"]) == 5
    assert len(board["cards"]) == 8
    assert board["columns"][0]["cardIds"] == ["card-1", "card-2"]


def test_board_requires_auth(tmp_path, monkeypatch):
    monkeypatch.setenv("PM_DB_PATH", str(tmp_path / "test.db"))
    from app.db import init_db

    init_db()
    from app.main import app

    anonymous = TestClient(app)
    assert anonymous.get("/api/board").status_code == 401


def test_rename_column(client):
    response = client.patch("/api/columns/col-backlog", json={"title": "Ideas"})
    assert response.status_code == 200
    board = response.json()
    assert board["columns"][0]["title"] == "Ideas"


def test_rename_unknown_column(client):
    response = client.patch("/api/columns/col-missing", json={"title": "Nope"})
    assert response.status_code == 404


def test_create_card(client):
    response = client.post(
        "/api/cards",
        json={"columnId": "col-review", "title": "New card", "details": "Notes"},
    )
    assert response.status_code == 200
    board = response.json()
    review = next(c for c in board["columns"] if c["id"] == "col-review")
    assert len(review["cardIds"]) == 2
    new_card_id = review["cardIds"][-1]
    assert board["cards"][new_card_id]["title"] == "New card"


def test_update_card(client):
    response = client.patch(
        "/api/cards/card-1", json={"title": "Updated", "details": "Fresh"}
    )
    assert response.status_code == 200
    board = response.json()
    assert board["cards"]["card-1"] == {
        "id": "card-1",
        "title": "Updated",
        "details": "Fresh",
    }


def test_delete_card(client):
    response = client.delete("/api/cards/card-1")
    assert response.status_code == 200
    board = response.json()
    assert "card-1" not in board["cards"]
    assert board["columns"][0]["cardIds"] == ["card-2"]


def test_move_card_between_columns(client):
    response = client.post(
        "/api/cards/card-1/move", json={"columnId": "col-done", "position": 0}
    )
    assert response.status_code == 200
    board = response.json()
    backlog = next(c for c in board["columns"] if c["id"] == "col-backlog")
    done = next(c for c in board["columns"] if c["id"] == "col-done")
    assert "card-1" not in backlog["cardIds"]
    assert done["cardIds"][0] == "card-1"


def test_move_card_within_column(client):
    response = client.post(
        "/api/cards/card-1/move", json={"columnId": "col-backlog", "position": 1}
    )
    assert response.status_code == 200
    board = response.json()
    backlog = next(c for c in board["columns"] if c["id"] == "col-backlog")
    assert backlog["cardIds"] == ["card-2", "card-1"]


def test_scoping_blocks_other_board(client):
    from app.db import get_connection

    conn = get_connection()
    conn.execute("INSERT INTO users (id, username) VALUES ('user-2', 'other')")
    conn.execute(
        "INSERT INTO boards (id, user_id, title) VALUES ('board-2', 'user-2', 'B2')"
    )
    conn.execute(
        "INSERT INTO columns (id, board_id, title, position) "
        "VALUES ('col-x', 'board-2', 'X', 0)"
    )
    conn.execute(
        "INSERT INTO cards (id, column_id, title, details, position) "
        "VALUES ('card-x', 'col-x', 'Secret', '', 0)"
    )
    conn.commit()
    conn.close()

    assert client.patch(
        "/api/cards/card-x", json={"title": "Hacked", "details": ""}
    ).status_code == 404
    assert client.delete("/api/cards/card-x").status_code == 404
    assert client.patch(
        "/api/columns/col-x", json={"title": "Hacked"}
    ).status_code == 404


def test_apply_actions_persists_a_move(client):
    from app import board as board_service
    from app.db import get_connection

    conn = get_connection()
    board_id = conn.execute("SELECT id FROM boards LIMIT 1").fetchone()["id"]
    board_service.apply_actions(
        conn,
        board_id,
        [{"type": "move_card", "cardId": "card-4", "columnId": "col-done", "position": 0}],
    )
    board = board_service.get_board(conn, board_id)
    conn.close()

    done = next(c for c in board["columns"] if c["id"] == "col-done")
    progress = next(c for c in board["columns"] if c["id"] == "col-progress")
    assert done["cardIds"][0] == "card-4"
    assert "card-4" not in progress["cardIds"]


def test_ai_chat_requires_auth(tmp_path, monkeypatch):
    monkeypatch.setenv("PM_DB_PATH", str(tmp_path / "test.db"))
    from app.db import init_db

    init_db()
    from app.main import app

    anonymous = TestClient(app)
    assert anonymous.post("/api/ai/chat", json={"question": "hi"}).status_code == 401


def test_ai_chat_applies_actions(client, monkeypatch):
    from app import ai

    def fake_chat(board, history, question):
        return ai.AIResponse(
            reply="Moved it.",
            actions=[
                ai.BoardAction(
                    type="move_card",
                    cardId="card-4",
                    columnId="col-done",
                    position=0,
                )
            ],
        )

    monkeypatch.setattr(ai, "chat", fake_chat)
    response = client.post("/api/ai/chat", json={"question": "move card-4 to done"})
    assert response.status_code == 200
    data = response.json()
    assert data["reply"] == "Moved it."
    done = next(c for c in data["board"]["columns"] if c["id"] == "col-done")
    assert done["cardIds"][0] == "card-4"


def test_ai_chat_missing_key_returns_503(client, monkeypatch):
    from app import ai

    def boom(*args, **kwargs):
        raise ai.MissingAPIKey()

    monkeypatch.setattr(ai, "chat", boom)
    response = client.post("/api/ai/chat", json={"question": "hi"})
    assert response.status_code == 503


def test_ai_chat_unknown_id_returns_400(client, monkeypatch):
    from app import ai

    def fake_chat(board, history, question):
        return ai.AIResponse(
            reply="Done.",
            actions=[ai.BoardAction(type="delete_card", cardId="card-missing")],
        )

    monkeypatch.setattr(ai, "chat", fake_chat)
    response = client.post("/api/ai/chat", json={"question": "delete a ghost"})
    assert response.status_code == 400
