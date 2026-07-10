from fastapi.testclient import TestClient

from app.main import app


def test_login_success():
    client = TestClient(app)
    response = client.post(
        "/api/login", json={"username": "user", "password": "password"}
    )
    assert response.status_code == 200
    assert response.json() == {"ok": True}


def test_login_failure():
    client = TestClient(app)
    response = client.post(
        "/api/login", json={"username": "user", "password": "wrong"}
    )
    assert response.status_code == 401


def test_me_without_session():
    client = TestClient(app)
    response = client.get("/api/me")
    assert response.status_code == 200
    assert response.json() == {"authenticated": False}


def test_me_with_session():
    client = TestClient(app)
    client.post("/api/login", json={"username": "user", "password": "password"})
    response = client.get("/api/me")
    assert response.json() == {"authenticated": True}


def test_logout_clears_session():
    client = TestClient(app)
    client.post("/api/login", json={"username": "user", "password": "password"})
    client.post("/api/logout")
    response = client.get("/api/me")
    assert response.json() == {"authenticated": False}
