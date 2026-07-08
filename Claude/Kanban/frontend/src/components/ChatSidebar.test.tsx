import { render, screen, waitFor } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { ChatSidebar } from "@/components/ChatSidebar";
import { sendChat } from "@/lib/api";

vi.mock("@/lib/api", () => ({ sendChat: vi.fn() }));

const mockedSendChat = vi.mocked(sendChat);

afterEach(() => {
  vi.clearAllMocks();
});

describe("ChatSidebar", () => {
  it("shows the user message and the assistant reply", async () => {
    mockedSendChat.mockResolvedValue({ reply: "Sure thing!", board_update: null });
    render(<ChatSidebar onBoardUpdate={vi.fn()} />);

    await userEvent.click(screen.getByRole("button", { name: /open assistant/i }));
    await userEvent.type(
      screen.getByLabelText(/message the assistant/i),
      "Hello there"
    );
    await userEvent.click(screen.getByRole("button", { name: /send/i }));

    expect(screen.getByText("Hello there")).toBeInTheDocument();
    expect(await screen.findByText("Sure thing!")).toBeInTheDocument();
    expect(mockedSendChat).toHaveBeenCalledWith("Hello there", []);
  });

  it("calls onBoardUpdate when the reply includes a board update", async () => {
    const board = { columns: [], cards: {} };
    mockedSendChat.mockResolvedValue({ reply: "Done", board_update: board });
    const onBoardUpdate = vi.fn();
    render(<ChatSidebar onBoardUpdate={onBoardUpdate} />);

    await userEvent.click(screen.getByRole("button", { name: /open assistant/i }));
    await userEvent.type(screen.getByLabelText(/message the assistant/i), "add a card");
    await userEvent.click(screen.getByRole("button", { name: /send/i }));

    await waitFor(() => expect(onBoardUpdate).toHaveBeenCalledWith(board));
  });
});
