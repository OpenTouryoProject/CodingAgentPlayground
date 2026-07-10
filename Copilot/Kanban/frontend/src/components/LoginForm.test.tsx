import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { afterEach, beforeEach, vi } from "vitest";
import { LoginForm } from "@/components/LoginForm";

describe("LoginForm", () => {
  beforeEach(() => {
    vi.restoreAllMocks();
  });

  afterEach(() => {
    vi.restoreAllMocks();
  });

  it("calls onSuccess after a successful login", async () => {
    const fetchMock = vi
      .fn()
      .mockResolvedValue({ ok: true, json: async () => ({ ok: true }) });
    vi.stubGlobal("fetch", fetchMock);
    const onSuccess = vi.fn();

    render(<LoginForm onSuccess={onSuccess} />);
    await userEvent.type(screen.getByLabelText("Username"), "user");
    await userEvent.type(screen.getByLabelText("Password"), "password");
    await userEvent.click(screen.getByRole("button", { name: /sign in/i }));

    expect(fetchMock).toHaveBeenCalledWith(
      "/api/login",
      expect.objectContaining({ method: "POST" })
    );
    expect(onSuccess).toHaveBeenCalled();
  });

  it("shows an error on failed login", async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok: false });
    vi.stubGlobal("fetch", fetchMock);
    const onSuccess = vi.fn();

    render(<LoginForm onSuccess={onSuccess} />);
    await userEvent.type(screen.getByLabelText("Username"), "user");
    await userEvent.type(screen.getByLabelText("Password"), "wrong");
    await userEvent.click(screen.getByRole("button", { name: /sign in/i }));

    expect(await screen.findByRole("alert")).toHaveTextContent(
      /invalid username or password/i
    );
    expect(onSuccess).not.toHaveBeenCalled();
  });
});
