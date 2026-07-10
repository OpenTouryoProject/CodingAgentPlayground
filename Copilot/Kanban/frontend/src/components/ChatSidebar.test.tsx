import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { ChatSidebar } from "@/components/ChatSidebar";
import { initialData } from "@/lib/kanban";

let fetchSpy: ReturnType<typeof vi.fn>;

beforeEach(() => {
  fetchSpy = vi.fn(async () => ({
    ok: true,
    json: async () => ({ reply: "Done!", board: initialData }),
  }));
  vi.stubGlobal("fetch", fetchSpy);
});

afterEach(() => {
  vi.unstubAllGlobals();
});

describe("ChatSidebar", () => {
  it("sends a message and renders the user question and reply", async () => {
    const onBoardUpdate = vi.fn();
    render(<ChatSidebar onBoardUpdate={onBoardUpdate} />);

    await userEvent.type(screen.getByLabelText("Message"), "hello there");
    await userEvent.click(screen.getByRole("button", { name: /send/i }));

    expect(await screen.findByText("Done!")).toBeInTheDocument();
    expect(screen.getByText("hello there")).toBeInTheDocument();
    expect(onBoardUpdate).toHaveBeenCalledWith(initialData);
    expect(fetchSpy).toHaveBeenCalledWith(
      "/api/ai/chat",
      expect.objectContaining({ method: "POST" })
    );
  });

  it("sends prior history with the next question", async () => {
    render(<ChatSidebar onBoardUpdate={() => {}} />);

    await userEvent.type(screen.getByLabelText("Message"), "first");
    await userEvent.click(screen.getByRole("button", { name: /send/i }));
    await screen.findByText("Done!");

    await userEvent.type(screen.getByLabelText("Message"), "second");
    await userEvent.click(screen.getByRole("button", { name: /send/i }));

    const body = JSON.parse(fetchSpy.mock.calls[1][1].body);
    expect(body.question).toBe("second");
    expect(body.history).toEqual([
      { role: "user", content: "first" },
      { role: "assistant", content: "Done!" },
    ]);
  });

  it("shows an error when the assistant call fails", async () => {
    fetchSpy.mockImplementationOnce(async () => ({ ok: false, status: 503 }));
    render(<ChatSidebar onBoardUpdate={() => {}} />);

    await userEvent.type(screen.getByLabelText("Message"), "hi");
    await userEvent.click(screen.getByRole("button", { name: /send/i }));

    expect(await screen.findByRole("alert")).toHaveTextContent(/unavailable/i);
  });
});
