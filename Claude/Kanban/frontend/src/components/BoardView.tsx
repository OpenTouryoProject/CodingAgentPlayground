"use client";

import { useCallback, useEffect, useRef, useState } from "react";
import { KanbanBoard } from "@/components/KanbanBoard";
import { ChatSidebar } from "@/components/ChatSidebar";
import { getBoard, saveBoard } from "@/lib/api";
import type { BoardData } from "@/lib/kanban";

const SAVE_DELAY_MS = 400;

export const BoardView = () => {
  const [board, setBoard] = useState<BoardData | null>(null);
  const [error, setError] = useState<string | null>(null);
  // Bumped to remount KanbanBoard when the AI replaces the board.
  const [version, setVersion] = useState(0);
  const saveTimer = useRef<ReturnType<typeof setTimeout> | null>(null);

  useEffect(() => {
    getBoard()
      .then(setBoard)
      .catch(() => setError("Could not load your board."));
  }, []);

  // Debounced save so rapid edits (e.g. typing a column name) collapse into one PUT.
  const handleBoardChange = useCallback((next: BoardData) => {
    if (saveTimer.current) {
      clearTimeout(saveTimer.current);
    }
    saveTimer.current = setTimeout(() => {
      saveBoard(next).catch(() => setError("Could not save your changes."));
    }, SAVE_DELAY_MS);
  }, []);

  // The AI already persisted the board server-side, so just swap it in and remount.
  const handleAiBoardUpdate = useCallback((next: BoardData) => {
    setBoard(next);
    setVersion((current) => current + 1);
  }, []);

  if (error) {
    return (
      <main className="grid min-h-screen place-items-center px-6">
        <p className="text-sm font-medium text-[#c0392b]">{error}</p>
      </main>
    );
  }

  if (!board) {
    return (
      <main className="grid min-h-screen place-items-center">
        <p className="text-sm font-semibold uppercase tracking-[0.3em] text-[var(--gray-text)]">
          Loading board...
        </p>
      </main>
    );
  }

  return (
    <>
      <KanbanBoard
        key={version}
        initialBoard={board}
        onBoardChange={handleBoardChange}
      />
      <ChatSidebar onBoardUpdate={handleAiBoardUpdate} />
    </>
  );
};
