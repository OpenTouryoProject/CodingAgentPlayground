import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { AddCardForm } from "@/components/AddCardForm";

describe("AddCardForm", () => {
  it("submits a new card", async () => {
    const user = userEvent.setup();
    const onAdd = vi.fn();

    render(<AddCardForm columnId="col-todo" onAdd={onAdd} />);

    await user.click(screen.getByTestId("add-card-toggle-col-todo"));
    await user.type(screen.getByTestId("add-card-title-col-todo"), "Test card");
    await user.type(
      screen.getByTestId("add-card-details-col-todo"),
      "Test details",
    );
    await user.click(screen.getByTestId("add-card-submit-col-todo"));

    expect(onAdd).toHaveBeenCalledWith("col-todo", "Test card", "Test details");
  });
});
