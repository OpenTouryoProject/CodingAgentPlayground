import json

import pytest
from fastapi.testclient import TestClient

from app import ai
from app.board import Card, initial_board
from app.main import app


@pytest.fixture
def db_env(tmp_path, monkeypatch):
    monkeypatch.setenv("APP_DB_PATH", str(tmp_path / "test.db"))
    yield


def _login(client: TestClient):
    res = client.post("/api/login", json={"username": "user", "password": "password"})
    assert res.status_code == 200


def _fake_reply(payload: dict):
    def _chat(messages, response_format=None):
        return json.dumps(payload)

    return _chat


def test_chat_requires_auth(db_env):
    with TestClient(app) as client:
        assert client.post("/api/chat", json={"message": "hi"}).status_code == 401


def test_chat_reply_only(db_env, monkeypatch):
    monkeypatch.setattr(ai, "chat", _fake_reply({"reply": "Hello!", "board_update": None}))
    with TestClient(app) as client:
        _login(client)
        res = client.post("/api/chat", json={"message": "hi"})
        assert res.status_code == 200
        assert res.json()["reply"] == "Hello!"
        assert res.json()["board_update"] is None
        # Board is untouched.
        assert len(client.get("/api/board").json()["columns"]) == 5


def test_chat_persists_board_update(db_env, monkeypatch):
    board = initial_board()
    board.cards["card-x"] = Card(id="card-x", title="From AI", details="added")
    board.columns[0].cardIds.append("card-x")
    monkeypatch.setattr(
        ai, "chat", _fake_reply({"reply": "Added it.", "board_update": board.model_dump()})
    )

    with TestClient(app) as client:
        _login(client)
        res = client.post("/api/chat", json={"message": "add a card"})
        assert res.status_code == 200
        assert res.json()["board_update"] is not None

        stored = client.get("/api/board").json()
        assert "card-x" in stored["cards"]
        assert "card-x" in stored["columns"][0]["cardIds"]


def test_chat_invalid_update_is_ignored(db_env, monkeypatch):
    monkeypatch.setattr(
        ai, "chat", _fake_reply({"reply": "hmm", "board_update": {"columns": "bad"}})
    )
    with TestClient(app) as client:
        _login(client)
        res = client.post("/api/chat", json={"message": "break it"})
        assert res.status_code == 200
        assert res.json()["board_update"] is None
        # Stored board is unchanged (still the seeded 5 columns).
        assert len(client.get("/api/board").json()["columns"]) == 5


def test_chat_invalid_input_422(db_env):
    with TestClient(app) as client:
        _login(client)
        assert client.post("/api/chat", json={}).status_code == 422
