import { render, screen } from "@testing-library/react";
import { describe, expect, it } from "vitest";
import { Board } from "@/components/Board";

describe("Board", () => {
  it("renders five columns and dummy cards", () => {
    render(<Board />);

    expect(screen.getByTestId("kanban-board")).toBeInTheDocument();
    expect(screen.getByTestId("column-col-backlog")).toBeInTheDocument();
    expect(screen.getByTestId("column-col-todo")).toBeInTheDocument();
    expect(screen.getByTestId("column-col-in-progress")).toBeInTheDocument();
    expect(screen.getByTestId("column-col-review")).toBeInTheDocument();
    expect(screen.getByTestId("column-col-done")).toBeInTheDocument();
    expect(screen.getByText("Research competitors")).toBeInTheDocument();
    expect(screen.getAllByTestId(/^card-/)).toHaveLength(10);
  });
});
