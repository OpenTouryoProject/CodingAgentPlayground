import {
  render,
  screen,
  within,
  waitFor,
} from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { KanbanBoard } from "@/components/KanbanBoard";
import {
  createId,
  initialData,
  updateCard,
  type BoardData,
} from "@/lib/kanban";

let board: BoardData;
let fetchSpy: ReturnType<typeof vi.fn>;

const json = (data: BoardData) => ({
  ok: true,
  json: async () => structuredClone(data),
});

const handleFetch = async (url: string, options?: RequestInit) => {
  const method = options?.method ?? "GET";
  const body = options?.body ? JSON.parse(options.body as string) : {};

  if (url === "/api/board") {
    return json(board);
  }

  if (url.startsWith("/api/columns/") && method === "PATCH") {
    const columnId = url.split("/api/columns/")[1];
    board = {
      ...board,
      columns: board.columns.map((column) =>
        column.id === columnId ? { ...column, title: body.title } : column
      ),
    };
    return json(board);
  }

  if (url === "/api/cards" && method === "POST") {
    const id = createId("card");
    board = {
      cards: { ...board.cards, [id]: { id, title: body.title, details: body.details } },
      columns: board.columns.map((column) =>
        column.id === body.columnId
          ? { ...column, cardIds: [...column.cardIds, id] }
          : column
      ),
    };
    return json(board);
  }

  if (url.startsWith("/api/cards/") && url.endsWith("/move") && method === "POST") {
    const cardId = url.split("/api/cards/")[1].replace("/move", "");
    const stripped = board.columns.map((column) => ({
      ...column,
      cardIds: column.cardIds.filter((id) => id !== cardId),
    }));
    board = {
      ...board,
      columns: stripped.map((column) =>
        column.id === body.columnId
          ? {
              ...column,
              cardIds: [
                ...column.cardIds.slice(0, body.position),
                cardId,
                ...column.cardIds.slice(body.position),
              ],
            }
          : column
      ),
    };
    return json(board);
  }

  if (url.startsWith("/api/cards/") && method === "PATCH") {
    const cardId = url.split("/api/cards/")[1];
    board = {
      ...board,
      cards: updateCard(board.cards, cardId, body.title, body.details),
    };
    return json(board);
  }

  if (url.startsWith("/api/cards/") && method === "DELETE") {
    const cardId = url.split("/api/cards/")[1];
    const rest = { ...board.cards };
    delete rest[cardId];
    board = {
      cards: rest,
      columns: board.columns.map((column) => ({
        ...column,
        cardIds: column.cardIds.filter((id) => id !== cardId),
      })),
    };
    return json(board);
  }

  if (url === "/api/ai/chat" && method === "POST") {
    const stripped = board.columns.map((column) => ({
      ...column,
      cardIds: column.cardIds.filter((id) => id !== "card-1"),
    }));
    board = {
      ...board,
      columns: stripped.map((column) =>
        column.id === "col-done"
          ? { ...column, cardIds: [...column.cardIds, "card-1"] }
          : column
      ),
    };
    return {
      ok: true,
      json: async () => ({
        reply: "Moved card-1 to Done.",
        board: structuredClone(board),
      }),
    };
  }

  throw new Error(`Unexpected request: ${method} ${url}`);
};

beforeEach(() => {
  board = structuredClone(initialData);
  fetchSpy = vi.fn((url: string, options?: RequestInit) =>
    handleFetch(url, options)
  );
  vi.stubGlobal("fetch", fetchSpy);
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("KanbanBoard", () => {
  it("loads and renders five columns from the API", async () => {
    render(<KanbanBoard onLogout={() => {}} />);
    await waitFor(() =>
      expect(screen.getAllByTestId(/column-/i)).toHaveLength(5)
    );
    expect(fetchSpy).toHaveBeenCalledWith("/api/board", expect.anything());
  });

  it("renames a column through the API", async () => {
    render(<KanbanBoard onLogout={() => {}} />);
    const column = await screen.findByTestId("column-col-backlog");
    const input = within(column).getByLabelText("Column title");
    await userEvent.clear(input);
    await userEvent.type(input, "New Name");
    await userEvent.tab();

    await waitFor(() =>
      expect(fetchSpy).toHaveBeenCalledWith(
        "/api/columns/col-backlog",
        expect.objectContaining({ method: "PATCH" })
      )
    );
    expect(input).toHaveValue("New Name");
  });

  it("adds and removes a card through the API", async () => {
    render(<KanbanBoard onLogout={() => {}} />);
    const column = await screen.findByTestId("column-col-backlog");

    await userEvent.click(
      within(column).getByRole("button", { name: /add a card/i })
    );
    await userEvent.type(
      within(column).getByPlaceholderText(/card title/i),
      "New card"
    );
    await userEvent.type(
      within(column).getByPlaceholderText(/details/i),
      "Notes"
    );
    await userEvent.click(
      within(column).getByRole("button", { name: /add card/i })
    );

    expect(await within(column).findByText("New card")).toBeInTheDocument();
    expect(fetchSpy).toHaveBeenCalledWith(
      "/api/cards",
      expect.objectContaining({ method: "POST" })
    );

    await userEvent.click(
      within(column).getByRole("button", { name: /delete new card/i })
    );

    await waitFor(() =>
      expect(within(column).queryByText("New card")).not.toBeInTheDocument()
    );
    expect(fetchSpy).toHaveBeenCalledWith(
      expect.stringMatching(/\/api\/cards\/card-/),
      expect.objectContaining({ method: "DELETE" })
    );
  });

  it("edits a card title and details through the API", async () => {
    render(<KanbanBoard onLogout={() => {}} />);
    const column = await screen.findByTestId("column-col-backlog");

    await userEvent.click(
      within(column).getByRole("button", { name: /edit align roadmap themes/i })
    );

    const titleInput = within(column).getByLabelText("Card title");
    await userEvent.clear(titleInput);
    await userEvent.type(titleInput, "Updated title");

    const detailsInput = within(column).getByLabelText("Card details");
    await userEvent.clear(detailsInput);
    await userEvent.type(detailsInput, "Updated details");

    await userEvent.click(within(column).getByRole("button", { name: /save/i }));

    expect(await within(column).findByText("Updated title")).toBeInTheDocument();
    expect(within(column).getByText("Updated details")).toBeInTheDocument();
    expect(fetchSpy).toHaveBeenCalledWith(
      "/api/cards/card-1",
      expect.objectContaining({ method: "PATCH" })
    );
  });

  it("shows an error when the board fails to load", async () => {
    fetchSpy.mockImplementationOnce(async () => ({ ok: false, status: 500 }));
    render(<KanbanBoard onLogout={() => {}} />);
    expect(
      await screen.findByText(/failed to load the board/i)
    ).toBeInTheDocument();
  });

  it("refreshes the board from an AI chat response", async () => {
    render(<KanbanBoard onLogout={() => {}} />);
    await screen.findByTestId("column-col-backlog");

    await userEvent.type(
      screen.getByLabelText("Message"),
      "move card-1 to done"
    );
    await userEvent.click(screen.getByRole("button", { name: /send/i }));

    expect(await screen.findByText("Moved card-1 to Done.")).toBeInTheDocument();
    const done = screen.getByTestId("column-col-done");
    await waitFor(() =>
      expect(within(done).getByTestId("card-card-1")).toBeInTheDocument()
    );
  });
});
