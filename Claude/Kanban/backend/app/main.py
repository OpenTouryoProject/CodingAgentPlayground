import os
from contextlib import asynccontextmanager
from pathlib import Path

from fastapi import Depends, FastAPI, HTTPException, Request
from fastapi.staticfiles import StaticFiles
from pydantic import BaseModel
from starlette.middleware.sessions import SessionMiddleware

from . import ai
from .ai import AiChatResult, ChatMessage
from .board import BoardData
from .db import get_board, init_db, save_board

STATIC_DIR = Path(__file__).parent / "static"

# MVP: single hardcoded user. The DB supports multiple users in later parts.
USERNAME = "user"
PASSWORD = "password"

# Signed-cookie session secret. Overridable via env for real deployments.
SESSION_SECRET = os.environ.get("SESSION_SECRET", "dev-session-secret")


@asynccontextmanager
async def lifespan(app: FastAPI):
    init_db()
    yield


app = FastAPI(title="Project Management MVP", lifespan=lifespan)
app.add_middleware(SessionMiddleware, secret_key=SESSION_SECRET, same_site="lax")


class Credentials(BaseModel):
    username: str
    password: str


def get_current_user(request: Request) -> str:
    user = request.session.get("user")
    if not user:
        raise HTTPException(status_code=401, detail="Not authenticated")
    return user


@app.get("/api/health")
def health():
    return {"status": "ok"}


@app.post("/api/login")
def login(credentials: Credentials, request: Request):
    if credentials.username != USERNAME or credentials.password != PASSWORD:
        raise HTTPException(status_code=401, detail="Invalid credentials")
    request.session["user"] = credentials.username
    return {"user": credentials.username}


@app.post("/api/logout")
def logout(request: Request):
    request.session.clear()
    return {"ok": True}


@app.get("/api/me")
def me(user: str = Depends(get_current_user)):
    return {"user": user}


@app.get("/api/board", response_model=BoardData)
def read_board(user: str = Depends(get_current_user)):
    return get_board(user)


@app.put("/api/board", response_model=BoardData)
def update_board(board: BoardData, user: str = Depends(get_current_user)):
    save_board(user, board)
    return board


class ChatRequest(BaseModel):
    message: str
    history: list[ChatMessage] = []


@app.post("/api/chat", response_model=AiChatResult)
def chat(request: ChatRequest, user: str = Depends(get_current_user)):
    board = get_board(user)
    try:
        result = ai.chat_about_board(board, request.message, request.history)
    except ai.AIError as exc:
        raise HTTPException(status_code=503, detail=str(exc))
    if result.board_update is not None:
        save_board(user, result.board_update)
    return result


# Serve the static site at /. API routes above are matched first; everything
# else falls through to the static files (the exported frontend in the image).
app.mount("/", StaticFiles(directory=STATIC_DIR, html=True), name="static")
