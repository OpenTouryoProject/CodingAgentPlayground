from unittest.mock import MagicMock

import pytest

from app import ai


def test_ask_builds_request_and_returns_reply(monkeypatch):
    message = MagicMock()
    message.content = "4"
    choice = MagicMock()
    choice.message = message
    completion = MagicMock()
    completion.choices = [choice]

    client = MagicMock()
    client.chat.completions.create.return_value = completion
    monkeypatch.setattr(ai, "get_client", lambda: client)

    reply = ai.ask("What is 2+2? Reply with the number only.")

    assert reply == "4"
    client.chat.completions.create.assert_called_once_with(
        model="openai/gpt-oss-120b",
        messages=[
            {
                "role": "user",
                "content": "What is 2+2? Reply with the number only.",
            }
        ],
    )


def test_get_client_requires_api_key(monkeypatch):
    monkeypatch.delenv("OPENROUTER_API_KEY", raising=False)
    with pytest.raises(ai.MissingAPIKey):
        ai.get_client()


def test_build_messages_includes_board_and_history():
    board = {"columns": [{"id": "col-x", "title": "X", "cardIds": []}], "cards": {}}
    history = [
        {"role": "user", "content": "hi"},
        {"role": "assistant", "content": "hello"},
    ]
    messages = ai.build_messages(board, history, "add a card")

    assert messages[0]["role"] == "system"
    assert "col-x" in messages[1]["content"]
    assert messages[2:4] == history
    assert messages[-1] == {"role": "user", "content": "add a card"}


def _fake_parse_client(parsed):
    message = MagicMock()
    message.parsed = parsed
    choice = MagicMock()
    choice.message = message
    completion = MagicMock()
    completion.choices = [choice]
    client = MagicMock()
    client.chat.completions.parse.return_value = completion
    return client


def test_chat_parses_reply_only(monkeypatch):
    parsed = ai.AIResponse(reply="All good", actions=[])
    monkeypatch.setattr(ai, "get_client", lambda: _fake_parse_client(parsed))

    result = ai.chat({"columns": [], "cards": {}}, [], "how are things?")

    assert result.reply == "All good"
    assert result.actions == []


def test_chat_parses_reply_with_action(monkeypatch):
    action = ai.BoardAction(
        type="move_card", cardId="card-4", columnId="col-done", position=0
    )
    parsed = ai.AIResponse(reply="Moved it.", actions=[action])
    monkeypatch.setattr(ai, "get_client", lambda: _fake_parse_client(parsed))

    result = ai.chat({"columns": [], "cards": {}}, [], "move card-4 to done")

    assert result.reply == "Moved it."
    assert result.actions[0].type == "move_card"
    assert result.actions[0].cardId == "card-4"
