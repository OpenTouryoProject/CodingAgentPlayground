import { render, screen } from "@testing-library/react";
import userEvent from "@testing-library/user-event";
import { describe, expect, it, vi } from "vitest";
import { DndContext } from "@dnd-kit/core";
import { KanbanCard } from "@/components/KanbanCard";

const card = {
  id: "card-test",
  title: "Test card",
  details: "Details here",
};

function renderCard(onDelete = vi.fn()) {
  return render(
    <DndContext>
      <KanbanCard card={card} onDelete={onDelete} />
    </DndContext>,
  );
}

describe("KanbanCard", () => {
  it("calls onDelete when delete is clicked", async () => {
    const user = userEvent.setup();
    const onDelete = vi.fn();

    renderCard(onDelete);
    await user.click(screen.getByTestId("delete-card-card-test"));

    expect(onDelete).toHaveBeenCalledWith("card-test");
  });
});
