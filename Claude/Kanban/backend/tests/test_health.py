from fastapi.testclient import TestClient

from app.main import app

client = TestClient(app)


def test_health():
    res = client.get("/api/health")
    assert res.status_code == 200
    assert res.json() == {"status": "ok"}


def test_static_mount_registered():
    # The frontend is built and placed in app/static by the Docker image, so its
    # contents are not present in unit tests. Assert the mount exists instead.
    assert any(getattr(route, "name", None) == "static" for route in app.routes)
