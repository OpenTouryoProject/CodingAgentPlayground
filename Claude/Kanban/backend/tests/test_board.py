import pytest
from fastapi.testclient import TestClient

from app.main import app


@pytest.fixture
def db_env(tmp_path, monkeypatch):
    # Isolate each test on its own SQLite file.
    monkeypatch.setenv("APP_DB_PATH", str(tmp_path / "test.db"))
    yield tmp_path


def _login(client: TestClient):
    res = client.post("/api/login", json={"username": "user", "password": "password"})
    assert res.status_code == 200


def test_board_requires_auth(db_env):
    with TestClient(app) as client:
        assert client.get("/api/board").status_code == 401
        assert client.put("/api/board", json={"columns": [], "cards": {}}).status_code == 401


def test_get_board_seeds_initial(db_env):
    with TestClient(app) as client:
        _login(client)
        res = client.get("/api/board")
        assert res.status_code == 200
        board = res.json()
        assert len(board["columns"]) == 5
        assert "card-1" in board["cards"]
        assert board["columns"][0]["cardIds"] == ["card-1", "card-2"]


def test_put_board_persists(db_env):
    with TestClient(app) as client:
        _login(client)
        board = client.get("/api/board").json()

        board["cards"]["card-new"] = {
            "id": "card-new",
            "title": "Added",
            "details": "via API",
        }
        board["columns"][0]["cardIds"].append("card-new")

        put = client.put("/api/board", json=board)
        assert put.status_code == 200

        reloaded = client.get("/api/board").json()
        assert "card-new" in reloaded["cards"]
        assert "card-new" in reloaded["columns"][0]["cardIds"]


def test_put_invalid_payload_422(db_env):
    with TestClient(app) as client:
        _login(client)
        assert client.put("/api/board", json={"columns": "nope"}).status_code == 422


def test_persists_across_restart(db_env):
    with TestClient(app) as client:
        _login(client)
        board = client.get("/api/board").json()
        board["columns"][0]["title"] = "Renamed"
        assert client.put("/api/board", json=board).status_code == 200

    # A fresh client (new session, same DB file) sees the persisted change.
    with TestClient(app) as client2:
        _login(client2)
        reloaded = client2.get("/api/board").json()
        assert reloaded["columns"][0]["title"] == "Renamed"
