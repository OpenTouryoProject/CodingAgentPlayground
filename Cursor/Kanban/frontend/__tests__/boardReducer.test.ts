import { describe, expect, it, vi } from "vitest";
import { boardReducer } from "@/lib/boardReducer";
import { initialBoardState } from "@/lib/dummyData";

describe("boardReducer", () => {
  it("renames a column", () => {
    const next = boardReducer(initialBoardState, {
      type: "RENAME_COLUMN",
      columnId: "col-backlog",
      title: "Ideas",
    });

    expect(next.columns.find((col) => col.id === "col-backlog")?.title).toBe(
      "Ideas",
    );
  });

  it("ignores empty rename values", () => {
    const next = boardReducer(initialBoardState, {
      type: "RENAME_COLUMN",
      columnId: "col-backlog",
      title: "   ",
    });

    expect(next.columns.find((col) => col.id === "col-backlog")?.title).toBe(
      "Backlog",
    );
  });

  it("adds a card to a column", () => {
    vi.stubGlobal("crypto", {
      randomUUID: () => "new-card-id",
    });

    const next = boardReducer(initialBoardState, {
      type: "ADD_CARD",
      columnId: "col-todo",
      title: "New task",
      details: "Some details",
    });

    expect(next.cards["new-card-id"]).toEqual({
      id: "new-card-id",
      title: "New task",
      details: "Some details",
    });
    expect(next.columns.find((col) => col.id === "col-todo")?.cardIds).toContain(
      "new-card-id",
    );

    vi.unstubAllGlobals();
  });

  it("rejects add card with empty title", () => {
    const next = boardReducer(initialBoardState, {
      type: "ADD_CARD",
      columnId: "col-todo",
      title: "  ",
      details: "",
    });

    expect(next).toEqual(initialBoardState);
  });

  it("deletes a card", () => {
    const next = boardReducer(initialBoardState, {
      type: "DELETE_CARD",
      cardId: "card-1",
    });

    expect(next.cards["card-1"]).toBeUndefined();
    expect(
      next.columns.find((col) => col.id === "col-backlog")?.cardIds,
    ).not.toContain("card-1");
  });

  it("deletes the last card in a column", () => {
    const next = boardReducer(initialBoardState, {
      type: "DELETE_CARD",
      cardId: "card-8",
    });

    expect(next.columns.find((col) => col.id === "col-review")?.cardIds).toEqual(
      [],
    );
  });

  it("moves a card within the same column", () => {
    const next = boardReducer(initialBoardState, {
      type: "MOVE_CARD",
      cardId: "card-1",
      fromColumnId: "col-backlog",
      toColumnId: "col-backlog",
      toIndex: 2,
    });

    expect(next.columns.find((col) => col.id === "col-backlog")?.cardIds).toEqual([
      "card-2",
      "card-3",
      "card-1",
    ]);
  });

  it("moves a card across columns", () => {
    const next = boardReducer(initialBoardState, {
      type: "MOVE_CARD",
      cardId: "card-1",
      fromColumnId: "col-backlog",
      toColumnId: "col-todo",
      toIndex: 1,
    });

    expect(
      next.columns.find((col) => col.id === "col-backlog")?.cardIds,
    ).toEqual(["card-2", "card-3"]);
    expect(next.columns.find((col) => col.id === "col-todo")?.cardIds).toEqual([
      "card-4",
      "card-1",
      "card-5",
    ]);
  });

  it("keeps five columns after all actions", () => {
    vi.stubGlobal("crypto", {
      randomUUID: () => "new-card-id",
    });

    let state = boardReducer(initialBoardState, {
      type: "ADD_CARD",
      columnId: "col-done",
      title: "Extra",
      details: "",
    });
    state = boardReducer(state, {
      type: "DELETE_CARD",
      cardId: "card-9",
    });

    expect(state.columns).toHaveLength(5);

    vi.unstubAllGlobals();
  });
});
