import json
import os
from typing import Literal, Optional

from dotenv import find_dotenv, load_dotenv
from openai import OpenAI
from pydantic import BaseModel

load_dotenv(find_dotenv(usecwd=True))

BASE_URL = "https://openrouter.ai/api/v1"
MODEL = "openai/gpt-oss-120b"

SYSTEM_PROMPT = (
    "You are a helpful assistant for a single Kanban board. You can answer "
    "questions about the board and change it when asked. Reply in a short, "
    "friendly sentence. When the user asks to change the board, return the "
    "matching actions; otherwise return an empty actions list. Only use column "
    "ids and card ids that already exist in the board JSON. Never invent ids for "
    "existing items. To add a card, use create_card with the target columnId. "
    "Positions are zero-based within a column."
)


class BoardAction(BaseModel):
    type: Literal[
        "create_card",
        "update_card",
        "move_card",
        "delete_card",
        "rename_column",
    ]
    cardId: Optional[str] = None
    columnId: Optional[str] = None
    title: Optional[str] = None
    details: Optional[str] = None
    position: Optional[int] = None


class AIResponse(BaseModel):
    reply: str
    actions: list[BoardAction]


class MissingAPIKey(RuntimeError):
    pass


def get_client() -> OpenAI:
    api_key = os.environ.get("OPENROUTER_API_KEY")
    if not api_key:
        raise MissingAPIKey("OPENROUTER_API_KEY is not set")
    return OpenAI(base_url=BASE_URL, api_key=api_key)


def build_messages(board: dict, history: list[dict], question: str) -> list[dict]:
    return [
        {"role": "system", "content": SYSTEM_PROMPT},
        {"role": "system", "content": "Current board JSON:\n" + json.dumps(board)},
        *history,
        {"role": "user", "content": question},
    ]


def ask(prompt: str) -> str:
    response = get_client().chat.completions.create(
        model=MODEL,
        messages=[{"role": "user", "content": prompt}],
    )
    return response.choices[0].message.content


def chat(board: dict, history: list[dict], question: str) -> AIResponse:
    completion = get_client().chat.completions.parse(
        model=MODEL,
        messages=build_messages(board, history, question),
        response_format=AIResponse,
    )
    return completion.choices[0].message.parsed
