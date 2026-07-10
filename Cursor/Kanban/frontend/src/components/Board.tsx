"use client";

import {
  DndContext,
  DragOverlay,
  PointerSensor,
  closestCorners,
  useSensor,
  useSensors,
  type DragEndEvent,
  type DragOverEvent,
  type DragStartEvent,
} from "@dnd-kit/core";
import { useReducer, useState } from "react";
import { boardReducer, findColumnByCardId } from "@/lib/boardReducer";
import { initialBoardState } from "@/lib/dummyData";
import { Column } from "./Column";
import { KanbanCardPreview } from "./KanbanCard";

export function Board() {
  const [state, dispatch] = useReducer(boardReducer, initialBoardState);
  const [activeCardId, setActiveCardId] = useState<string | null>(null);

  const sensors = useSensors(
    useSensor(PointerSensor, {
      activationConstraint: { distance: 8 },
    }),
  );

  const activeCard = activeCardId ? state.cards[activeCardId] : null;

  function handleDragStart(event: DragStartEvent) {
    setActiveCardId(String(event.active.id));
  }

  function handleDragOver(event: DragOverEvent) {
    const { active, over } = event;
    if (!over || active.id === over.id) return;

    const cardId = String(active.id);
    const overId = String(over.id);
    const activeColumn = findColumnByCardId(state, cardId);
    const overColumn =
      state.columns.find((column) => column.id === overId) ??
      findColumnByCardId(state, overId);

    if (!activeColumn || !overColumn || activeColumn.id === overColumn.id) {
      return;
    }

    dispatch({
      type: "MOVE_CARD",
      cardId,
      fromColumnId: activeColumn.id,
      toColumnId: overColumn.id,
      toIndex: state.columns.some((column) => column.id === overId)
        ? overColumn.cardIds.length
        : overColumn.cardIds.indexOf(overId),
    });
  }

  function handleDragEnd(event: DragEndEvent) {
    const { active, over } = event;
    setActiveCardId(null);

    if (!over) return;

    const cardId = String(active.id);
    const overId = String(over.id);
    const activeColumn = findColumnByCardId(state, cardId);
    if (!activeColumn) return;

    const overColumn =
      state.columns.find((column) => column.id === overId) ??
      findColumnByCardId(state, overId);
    if (!overColumn) return;

    const toIndex = state.columns.some((column) => column.id === overId)
      ? overColumn.cardIds.length
      : overColumn.cardIds.indexOf(overId);

    if (activeColumn.id === overColumn.id) {
      const fromIndex = activeColumn.cardIds.indexOf(cardId);
      if (fromIndex !== toIndex) {
        dispatch({
          type: "MOVE_CARD",
          cardId,
          fromColumnId: activeColumn.id,
          toColumnId: overColumn.id,
          toIndex,
        });
      }
    }
  }

  return (
    <div className="flex min-h-screen flex-col">
      <header className="border-b border-accent-yellow/30 bg-white px-6 py-5 shadow-sm">
        <div className="mx-auto flex max-w-[1600px] items-center gap-3">
          <div className="h-8 w-1 rounded-full bg-accent-yellow" />
          <div>
            <h1 className="text-2xl font-bold text-navy-dark">Project Board</h1>
            <p className="text-sm text-gray-text">
              Drag cards between columns to track progress
            </p>
          </div>
        </div>
      </header>

      <DndContext
        sensors={sensors}
        collisionDetection={closestCorners}
        onDragStart={handleDragStart}
        onDragOver={handleDragOver}
        onDragEnd={handleDragEnd}
      >
        <main className="flex-1 overflow-x-auto px-6 py-6">
          <div
            className="mx-auto flex max-w-[1600px] gap-4"
            data-testid="kanban-board"
          >
            {state.columns.map((column) => (
              <Column
                key={column.id}
                column={column}
                cards={column.cardIds.map((cardId) => state.cards[cardId]).filter(Boolean)}
                onRename={(columnId, title) =>
                  dispatch({ type: "RENAME_COLUMN", columnId, title })
                }
                onAddCard={(columnId, title, details) =>
                  dispatch({ type: "ADD_CARD", columnId, title, details })
                }
                onDeleteCard={(cardId) =>
                  dispatch({ type: "DELETE_CARD", cardId })
                }
              />
            ))}
          </div>
        </main>

        <DragOverlay>
          {activeCard ? <KanbanCardPreview card={activeCard} /> : null}
        </DragOverlay>
      </DndContext>
    </div>
  );
}
