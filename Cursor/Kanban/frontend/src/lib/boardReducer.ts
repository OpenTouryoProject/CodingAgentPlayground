import type { BoardAction, BoardState } from "./types";

export function boardReducer(state: BoardState, action: BoardAction): BoardState {
  switch (action.type) {
    case "RENAME_COLUMN":
      return {
        ...state,
        columns: state.columns.map((column) =>
          column.id === action.columnId
            ? { ...column, title: action.title.trim() || column.title }
            : column,
        ),
      };

    case "ADD_CARD": {
      const title = action.title.trim();
      if (!title) return state;

      const id = crypto.randomUUID();
      const column = state.columns.find((col) => col.id === action.columnId);
      if (!column) return state;

      return {
        ...state,
        cards: {
          ...state.cards,
          [id]: { id, title, details: action.details.trim() },
        },
        columns: state.columns.map((col) =>
          col.id === action.columnId
            ? { ...col, cardIds: [...col.cardIds, id] }
            : col,
        ),
      };
    }

    case "DELETE_CARD": {
      const { cardId } = action;
      if (!state.cards[cardId]) return state;

      const remainingCards = { ...state.cards };
      delete remainingCards[cardId];

      return {
        cards: remainingCards,
        columns: state.columns.map((column) => ({
          ...column,
          cardIds: column.cardIds.filter((id) => id !== cardId),
        })),
      };
    }

    case "MOVE_CARD": {
      const { cardId, fromColumnId, toColumnId, toIndex } = action;
      const fromColumn = state.columns.find((col) => col.id === fromColumnId);
      const toColumn = state.columns.find((col) => col.id === toColumnId);
      if (!fromColumn || !toColumn || !fromColumn.cardIds.includes(cardId)) {
        return state;
      }

      if (fromColumnId === toColumnId) {
        const oldIndex = fromColumn.cardIds.indexOf(cardId);
        if (oldIndex === toIndex) return state;

        const reordered = [...fromColumn.cardIds];
        reordered.splice(oldIndex, 1);
        reordered.splice(toIndex, 0, cardId);

        return {
          ...state,
          columns: state.columns.map((column) =>
            column.id === fromColumnId ? { ...column, cardIds: reordered } : column,
          ),
        };
      }

      const sourceCardIds = fromColumn.cardIds.filter((id) => id !== cardId);
      const targetCardIds = [...toColumn.cardIds];
      targetCardIds.splice(toIndex, 0, cardId);

      return {
        ...state,
        columns: state.columns.map((column) => {
          if (column.id === fromColumnId) {
            return { ...column, cardIds: sourceCardIds };
          }
          if (column.id === toColumnId) {
            return { ...column, cardIds: targetCardIds };
          }
          return column;
        }),
      };
    }

    default:
      return state;
  }
}

export function findColumnByCardId(state: BoardState, cardId: string) {
  return state.columns.find((column) => column.cardIds.includes(cardId));
}
