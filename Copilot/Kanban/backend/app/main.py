import os
from contextlib import asynccontextmanager
from pathlib import Path

from fastapi import Depends, FastAPI, HTTPException, Request
from fastapi.staticfiles import StaticFiles
from pydantic import BaseModel
from starlette.middleware.sessions import SessionMiddleware

from app import ai
from app import board as board_service
from app.db import get_connection, init_db


@asynccontextmanager
async def lifespan(app: FastAPI):
    init_db()
    yield


app = FastAPI(title="Project Management MVP", lifespan=lifespan)

app.add_middleware(
    SessionMiddleware,
    secret_key=os.environ.get("SESSION_SECRET", "dev-secret"),
)

STATIC_DIR = Path(__file__).parent / "static"

USERNAME = "user"
PASSWORD = "password"


class Credentials(BaseModel):
    username: str
    password: str


class ColumnRename(BaseModel):
    title: str


class CardCreate(BaseModel):
    columnId: str
    title: str
    details: str = ""


class CardUpdate(BaseModel):
    title: str
    details: str = ""


class CardMove(BaseModel):
    columnId: str
    position: int


class AIPrompt(BaseModel):
    prompt: str


class ChatMessage(BaseModel):
    role: str
    content: str


class ChatRequest(BaseModel):
    question: str
    history: list[ChatMessage] = []


def current_board_id(request: Request) -> str:
    username = request.session.get("user")
    if not username:
        raise HTTPException(status_code=401, detail="Not authenticated")
    conn = get_connection()
    try:
        row = conn.execute(
            "SELECT boards.id AS id FROM boards "
            "JOIN users ON boards.user_id = users.id WHERE users.username = ?",
            (username,),
        ).fetchone()
    finally:
        conn.close()
    if row is None:
        raise HTTPException(status_code=404, detail="Board not found")
    return row["id"]


@app.get("/api/health")
def health():
    return {"status": "ok"}


@app.post("/api/login")
def login(credentials: Credentials, request: Request):
    if credentials.username == USERNAME and credentials.password == PASSWORD:
        request.session["user"] = credentials.username
        return {"ok": True}
    raise HTTPException(status_code=401, detail="Invalid credentials")


@app.post("/api/logout")
def logout(request: Request):
    request.session.clear()
    return {"ok": True}


@app.get("/api/me")
def me(request: Request):
    return {"authenticated": "user" in request.session}


@app.get("/api/board")
def read_board(board_id: str = Depends(current_board_id)):
    conn = get_connection()
    try:
        return board_service.get_board(conn, board_id)
    finally:
        conn.close()


@app.patch("/api/columns/{column_id}")
def patch_column(
    column_id: str,
    body: ColumnRename,
    board_id: str = Depends(current_board_id),
):
    conn = get_connection()
    try:
        try:
            board_service.rename_column(conn, board_id, column_id, body.title)
        except board_service.NotFound:
            raise HTTPException(status_code=404, detail="Column not found")
        return board_service.get_board(conn, board_id)
    finally:
        conn.close()


@app.post("/api/cards")
def post_card(body: CardCreate, board_id: str = Depends(current_board_id)):
    conn = get_connection()
    try:
        try:
            board_service.create_card(
                conn, board_id, body.columnId, body.title, body.details
            )
        except board_service.NotFound:
            raise HTTPException(status_code=404, detail="Column not found")
        return board_service.get_board(conn, board_id)
    finally:
        conn.close()


@app.patch("/api/cards/{card_id}")
def patch_card(
    card_id: str,
    body: CardUpdate,
    board_id: str = Depends(current_board_id),
):
    conn = get_connection()
    try:
        try:
            board_service.update_card(
                conn, board_id, card_id, body.title, body.details
            )
        except board_service.NotFound:
            raise HTTPException(status_code=404, detail="Card not found")
        return board_service.get_board(conn, board_id)
    finally:
        conn.close()


@app.post("/api/cards/{card_id}/move")
def move_card(
    card_id: str,
    body: CardMove,
    board_id: str = Depends(current_board_id),
):
    conn = get_connection()
    try:
        try:
            board_service.move_card(
                conn, board_id, card_id, body.columnId, body.position
            )
        except board_service.NotFound:
            raise HTTPException(status_code=404, detail="Card or column not found")
        return board_service.get_board(conn, board_id)
    finally:
        conn.close()


@app.delete("/api/cards/{card_id}")
def remove_card(card_id: str, board_id: str = Depends(current_board_id)):
    conn = get_connection()
    try:
        try:
            board_service.delete_card(conn, board_id, card_id)
        except board_service.NotFound:
            raise HTTPException(status_code=404, detail="Card not found")
        return board_service.get_board(conn, board_id)
    finally:
        conn.close()


@app.post("/api/ai/ask")
def ai_ask(body: AIPrompt, board_id: str = Depends(current_board_id)):
    try:
        reply = ai.ask(body.prompt)
    except ai.MissingAPIKey:
        raise HTTPException(status_code=503, detail="AI is not configured")
    return {"reply": reply}


@app.post("/api/ai/chat")
def ai_chat(body: ChatRequest, board_id: str = Depends(current_board_id)):
    conn = get_connection()
    try:
        board = board_service.get_board(conn, board_id)
        history = [message.model_dump() for message in body.history]
        try:
            result = ai.chat(board, history, body.question)
        except ai.MissingAPIKey:
            raise HTTPException(status_code=503, detail="AI is not configured")
        try:
            board_service.apply_actions(
                conn, board_id, [action.model_dump() for action in result.actions]
            )
        except board_service.NotFound:
            raise HTTPException(
                status_code=400,
                detail="The AI referenced a card or column that does not exist",
            )
        return {"reply": result.reply, "board": board_service.get_board(conn, board_id)}
    finally:
        conn.close()


app.mount("/", StaticFiles(directory=STATIC_DIR, html=True), name="static")
