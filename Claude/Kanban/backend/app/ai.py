import json
import os

import httpx
from pydantic import BaseModel, ValidationError

from .board import BoardData

OPENROUTER_URL = "https://openrouter.ai/api/v1/chat/completions"
MODEL = "openai/gpt-oss-120b"


class AIError(RuntimeError):
    pass


class ChatMessage(BaseModel):
    role: str  # "user" or "assistant"
    content: str


class AiChatResult(BaseModel):
    reply: str
    board_update: BoardData | None = None


def _api_key() -> str:
    key = os.environ.get("OPENROUTER_API_KEY")
    if not key:
        raise AIError("OPENROUTER_API_KEY is not set.")
    return key


def chat(messages: list[dict], response_format: dict | None = None) -> str:
    """Send a chat completion request to OpenRouter and return the reply text."""
    payload: dict = {"model": MODEL, "messages": messages}
    if response_format is not None:
        payload["response_format"] = response_format

    headers = {
        "Authorization": f"Bearer {_api_key()}",
        "Content-Type": "application/json",
    }

    with httpx.Client(timeout=60) as client:
        res = client.post(OPENROUTER_URL, headers=headers, json=payload)
    res.raise_for_status()
    return res.json()["choices"][0]["message"]["content"]


SYSTEM_PROMPT = """You are an assistant inside a Kanban board app. You receive the current \
board as JSON and the user's message. Reply with a single JSON object with exactly these \
keys:
- "reply": a short, friendly natural-language response to the user.
- "board_update": null when no board change is needed, or the FULL updated board when the \
user asks to create, edit, move, or delete cards or columns.

When updating, return the entire board (every column and card), not a diff. Keep existing \
ids; invent new string ids for new cards or columns. The board JSON shape is:
{"columns": [{"id": str, "title": str, "cardIds": [str, ...]}], \
"cards": {"<cardId>": {"id": str, "title": str, "details": str}}}
Every id listed in a column's cardIds must exist in "cards". Do not add any other keys."""


def chat_about_board(
    board: BoardData, message: str, history: list[ChatMessage]
) -> AiChatResult:
    """Ask the model about the board; return a reply and an optional board update."""
    messages: list[dict] = [
        {"role": "system", "content": SYSTEM_PROMPT},
        {"role": "system", "content": "Current board JSON:\n" + board.model_dump_json()},
    ]
    for item in history:
        messages.append({"role": item.role, "content": item.content})
    messages.append({"role": "user", "content": message})

    content = chat(messages, response_format={"type": "json_object"})

    try:
        data = json.loads(content)
    except json.JSONDecodeError as exc:
        raise AIError("Model did not return valid JSON.") from exc

    reply = str(data.get("reply", ""))

    # An invalid board_update is dropped (not persisted) so a bad suggestion never
    # corrupts the stored board; the reply is still returned.
    board_update = None
    raw_update = data.get("board_update")
    if raw_update is not None:
        try:
            board_update = BoardData.model_validate(raw_update)
        except ValidationError:
            board_update = None

    return AiChatResult(reply=reply, board_update=board_update)
