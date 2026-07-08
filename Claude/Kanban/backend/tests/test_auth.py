from fastapi.testclient import TestClient

from app.main import app


def test_me_requires_auth():
    client = TestClient(app)
    res = client.get("/api/me")
    assert res.status_code == 401


def test_login_success_sets_session():
    client = TestClient(app)
    res = client.post("/api/login", json={"username": "user", "password": "password"})
    assert res.status_code == 200
    assert res.json() == {"user": "user"}
    assert "session" in res.cookies

    # The session cookie now authenticates /api/me
    me = client.get("/api/me")
    assert me.status_code == 200
    assert me.json() == {"user": "user"}


def test_login_failure():
    client = TestClient(app)
    res = client.post("/api/login", json={"username": "user", "password": "wrong"})
    assert res.status_code == 401
    assert client.get("/api/me").status_code == 401


def test_logout_clears_session():
    client = TestClient(app)
    client.post("/api/login", json={"username": "user", "password": "password"})
    assert client.get("/api/me").status_code == 200

    res = client.post("/api/logout")
    assert res.status_code == 200
    assert client.get("/api/me").status_code == 401
