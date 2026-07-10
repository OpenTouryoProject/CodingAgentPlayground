import type { BoardState } from "./types";

export const initialBoardState: BoardState = {
  columns: [
    { id: "col-backlog", title: "Backlog", cardIds: ["card-1", "card-2", "card-3"] },
    { id: "col-todo", title: "To Do", cardIds: ["card-4", "card-5"] },
    { id: "col-in-progress", title: "In Progress", cardIds: ["card-6", "card-7"] },
    { id: "col-review", title: "Review", cardIds: ["card-8"] },
    { id: "col-done", title: "Done", cardIds: ["card-9", "card-10"] },
  ],
  cards: {
    "card-1": {
      id: "card-1",
      title: "Research competitors",
      details: "Review top 5 kanban tools and note standout UX patterns.",
    },
    "card-2": {
      id: "card-2",
      title: "Define color palette",
      details: "Finalize brand colors for headings, accents, and actions.",
    },
    "card-3": {
      id: "card-3",
      title: "Sketch board layout",
      details: "Wireframe the five-column layout with card placement.",
    },
    "card-4": {
      id: "card-4",
      title: "Set up project scaffold",
      details: "Initialize Next.js app with TypeScript and Tailwind.",
    },
    "card-5": {
      id: "card-5",
      title: "Build column components",
      details: "Create reusable column and card UI with rename support.",
    },
    "card-6": {
      id: "card-6",
      title: "Implement drag and drop",
      details: "Integrate dnd-kit for moving cards between columns.",
    },
    "card-7": {
      id: "card-7",
      title: "Add card form",
      details: "Inline form at column bottom for new cards.",
    },
    "card-8": {
      id: "card-8",
      title: "Write unit tests",
      details: "Cover reducer actions and key component interactions.",
    },
    "card-9": {
      id: "card-9",
      title: "Polish visual design",
      details: "Refine spacing, shadows, and hover states.",
    },
    "card-10": {
      id: "card-10",
      title: "Ship MVP",
      details: "Run E2E tests and confirm all flows work.",
    },
  },
};
