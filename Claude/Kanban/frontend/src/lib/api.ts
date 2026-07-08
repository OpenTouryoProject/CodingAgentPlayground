// Client helpers for the backend API. The frontend is served by the backend at
// the same origin, so cookies are sent automatically.

import type { BoardData } from "@/lib/kanban";

export async function getMe(): Promise<string | null> {
  const res = await fetch("/api/me");
  if (!res.ok) {
    return null;
  }
  const data = await res.json();
  return data.user as string;
}

export async function login(username: string, password: string): Promise<void> {
  const res = await fetch("/api/login", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ username, password }),
  });
  if (!res.ok) {
    throw new Error("Invalid username or password.");
  }
}

export async function logout(): Promise<void> {
  await fetch("/api/logout", { method: "POST" });
}

export async function getBoard(): Promise<BoardData> {
  const res = await fetch("/api/board");
  if (!res.ok) {
    throw new Error("Failed to load board.");
  }
  return res.json();
}

export async function saveBoard(board: BoardData): Promise<void> {
  const res = await fetch("/api/board", {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(board),
  });
  if (!res.ok) {
    throw new Error("Failed to save board.");
  }
}

export type ChatMessage = { role: "user" | "assistant"; content: string };

export type ChatResult = { reply: string; board_update: BoardData | null };

export async function sendChat(
  message: string,
  history: ChatMessage[]
): Promise<ChatResult> {
  const res = await fetch("/api/chat", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ message, history }),
  });
  if (!res.ok) {
    throw new Error("The assistant could not respond.");
  }
  return res.json();
}
