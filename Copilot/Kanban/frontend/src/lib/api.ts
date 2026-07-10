import type { BoardData } from "@/lib/kanban";

const request = async (
  url: string,
  options?: RequestInit
): Promise<BoardData> => {
  const response = await fetch(url, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });
  if (!response.ok) {
    throw new Error(`Request failed: ${response.status}`);
  }
  return response.json();
};

export const fetchBoard = () => request("/api/board");

export const renameColumnApi = (columnId: string, title: string) =>
  request(`/api/columns/${columnId}`, {
    method: "PATCH",
    body: JSON.stringify({ title }),
  });

export const createCardApi = (
  columnId: string,
  title: string,
  details: string
) =>
  request("/api/cards", {
    method: "POST",
    body: JSON.stringify({ columnId, title, details }),
  });

export const updateCardApi = (cardId: string, title: string, details: string) =>
  request(`/api/cards/${cardId}`, {
    method: "PATCH",
    body: JSON.stringify({ title, details }),
  });

export const moveCardApi = (
  cardId: string,
  columnId: string,
  position: number
) =>
  request(`/api/cards/${cardId}/move`, {
    method: "POST",
    body: JSON.stringify({ columnId, position }),
  });

export const deleteCardApi = (cardId: string) =>
  request(`/api/cards/${cardId}`, { method: "DELETE" });

export type ChatMessage = { role: "user" | "assistant"; content: string };
export type ChatResponse = { reply: string; board: BoardData };

export const chatApi = (
  question: string,
  history: ChatMessage[]
): Promise<ChatResponse> =>
  fetch("/api/ai/chat", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ question, history }),
  }).then((response) => {
    if (!response.ok) {
      throw new Error(`Request failed: ${response.status}`);
    }
    return response.json();
  });
