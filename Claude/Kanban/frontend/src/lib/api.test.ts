import { getBoard, saveBoard } from "@/lib/api";
import type { BoardData } from "@/lib/kanban";

const emptyBoard: BoardData = { columns: [], cards: {} };

afterEach(() => {
  vi.restoreAllMocks();
});

describe("api board helpers", () => {
  it("getBoard returns the parsed board", async () => {
    vi.stubGlobal(
      "fetch",
      vi.fn().mockResolvedValue({ ok: true, json: async () => emptyBoard })
    );
    await expect(getBoard()).resolves.toEqual(emptyBoard);
  });

  it("getBoard throws on a failed response", async () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue({ ok: false }));
    await expect(getBoard()).rejects.toThrow(/board/i);
  });

  it("saveBoard PUTs the board as JSON", async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: true });
    vi.stubGlobal("fetch", fetchMock);

    await saveBoard(emptyBoard);

    expect(fetchMock).toHaveBeenCalledWith(
      "/api/board",
      expect.objectContaining({
        method: "PUT",
        body: JSON.stringify(emptyBoard),
      })
    );
  });

  it("saveBoard throws on a failed response", async () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue({ ok: false }));
    await expect(saveBoard(emptyBoard)).rejects.toThrow(/save/i);
  });
});
