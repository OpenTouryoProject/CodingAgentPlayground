import { moveCard, updateCard, type Column, type Card } from "@/lib/kanban";

describe("moveCard", () => {
  const baseColumns: Column[] = [
    { id: "col-a", title: "A", cardIds: ["card-1", "card-2"] },
    { id: "col-b", title: "B", cardIds: ["card-3"] },
  ];

  it("reorders cards in the same column", () => {
    const result = moveCard(baseColumns, "card-2", "card-1");
    expect(result[0].cardIds).toEqual(["card-2", "card-1"]);
  });

  it("moves cards to another column", () => {
    const result = moveCard(baseColumns, "card-2", "card-3");
    expect(result[0].cardIds).toEqual(["card-1"]);
    expect(result[1].cardIds).toEqual(["card-2", "card-3"]);
  });

  it("drops cards to the end of a column", () => {
    const result = moveCard(baseColumns, "card-1", "col-b");
    expect(result[0].cardIds).toEqual(["card-2"]);
    expect(result[1].cardIds).toEqual(["card-3", "card-1"]);
  });
});

describe("updateCard", () => {
  const cards: Record<string, Card> = {
    "card-1": { id: "card-1", title: "Old", details: "Old details" },
  };

  it("updates the title and details", () => {
    const result = updateCard(cards, "card-1", "New", "New details");
    expect(result["card-1"]).toEqual({
      id: "card-1",
      title: "New",
      details: "New details",
    });
  });

  it("returns cards unchanged for an unknown id", () => {
    const result = updateCard(cards, "missing", "New", "New details");
    expect(result).toBe(cards);
  });
});
