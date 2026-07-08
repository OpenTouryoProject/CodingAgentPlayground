import os

import httpx
import pytest

from app import ai
from app.board import initial_board


def test_chat_missing_key_raises(monkeypatch):
    monkeypatch.delenv("OPENROUTER_API_KEY", raising=False)
    with pytest.raises(ai.AIError):
        ai.chat([{"role": "user", "content": "hi"}])


def test_chat_parses_reply(monkeypatch):
    monkeypatch.setenv("OPENROUTER_API_KEY", "test-key")

    def fake_post(self, url, headers, json):
        assert json["model"] == ai.MODEL
        request = httpx.Request("POST", url)
        return httpx.Response(
            200,
            json={"choices": [{"message": {"content": "4"}}]},
            request=request,
        )

    monkeypatch.setattr(httpx.Client, "post", fake_post)
    assert ai.chat([{"role": "user", "content": "2+2?"}]) == "4"


@pytest.mark.skipif(
    not os.environ.get("OPENROUTER_API_KEY"),
    reason="live OpenRouter call; set OPENROUTER_API_KEY to run",
)
def test_chat_2plus2_live():
    reply = ai.chat(
        [{"role": "user", "content": "What is 2+2? Reply with just the number."}]
    )
    assert "4" in reply


@pytest.mark.skipif(
    not os.environ.get("OPENROUTER_API_KEY"),
    reason="live OpenRouter call; set OPENROUTER_API_KEY to run",
)
def test_chat_about_board_live():
    board = initial_board()
    before = len(board.cards)
    result = ai.chat_about_board(
        board,
        "Add a new card titled 'Buy milk' to the Backlog column.",
        [],
    )
    assert result.reply
    assert result.board_update is not None
    assert len(result.board_update.cards) >= before + 1
