export type Card = {
  id: string;
  title: string;
  details: string;
};

export type Column = {
  id: string;
  title: string;
  cardIds: string[];
};

export type BoardState = {
  columns: Column[];
  cards: Record<string, Card>;
};

export type BoardAction =
  | { type: "RENAME_COLUMN"; columnId: string; title: string }
  | { type: "ADD_CARD"; columnId: string; title: string; details: string }
  | { type: "DELETE_CARD"; cardId: string }
  | {
      type: "MOVE_CARD";
      cardId: string;
      fromColumnId: string;
      toColumnId: string;
      toIndex: number;
    };
