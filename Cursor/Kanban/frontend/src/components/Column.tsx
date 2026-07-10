"use client";

import { useDroppable } from "@dnd-kit/core";
import {
  SortableContext,
  verticalListSortingStrategy,
} from "@dnd-kit/sortable";
import { useEffect, useRef, useState } from "react";
import type { Card, Column as ColumnType } from "@/lib/types";
import { AddCardForm } from "./AddCardForm";
import { KanbanCard } from "./KanbanCard";

type ColumnProps = {
  column: ColumnType;
  cards: Card[];
  onRename: (columnId: string, title: string) => void;
  onAddCard: (columnId: string, title: string, details: string) => void;
  onDeleteCard: (cardId: string) => void;
};

export function Column({
  column,
  cards,
  onRename,
  onAddCard,
  onDeleteCard,
}: ColumnProps) {
  const [isEditing, setIsEditing] = useState(false);
  const [draftTitle, setDraftTitle] = useState(column.title);
  const inputRef = useRef<HTMLInputElement>(null);

  const { setNodeRef, isOver } = useDroppable({ id: column.id });

  useEffect(() => {
    if (isEditing) {
      inputRef.current?.focus();
      inputRef.current?.select();
    }
  }, [isEditing]);

  function startEditing() {
    setDraftTitle(column.title);
    setIsEditing(true);
  }

  function commitRename() {
    onRename(column.id, draftTitle);
    setIsEditing(false);
  }

  return (
    <section
      className="flex w-72 shrink-0 flex-col rounded-xl bg-white/80 shadow-sm ring-1 ring-gray-200/80"
      data-testid={`column-${column.id}`}
    >
      <div className="border-t-4 border-accent-yellow rounded-t-xl px-4 pt-4 pb-2">
        {isEditing ? (
          <input
            ref={inputRef}
            value={draftTitle}
            onChange={(event) => setDraftTitle(event.target.value)}
            onBlur={commitRename}
            onKeyDown={(event) => {
              if (event.key === "Enter") commitRename();
              if (event.key === "Escape") {
                setDraftTitle(column.title);
                setIsEditing(false);
              }
            }}
            className="w-full rounded border border-blue-primary/30 bg-white px-2 py-1 text-sm font-semibold text-navy-dark outline-none focus:border-blue-primary focus:ring-1 focus:ring-blue-primary"
            data-testid={`column-rename-input-${column.id}`}
          />
        ) : (
          <button
            type="button"
            onClick={startEditing}
            className="w-full text-left text-sm font-semibold tracking-wide text-navy-dark transition-colors hover:text-blue-primary"
            data-testid={`column-title-${column.id}`}
          >
            {column.title}
          </button>
        )}
        <p className="mt-1 text-xs text-gray-text">{cards.length} cards</p>
      </div>

      <div
        ref={setNodeRef}
        className={`flex min-h-[120px] flex-1 flex-col px-3 pb-3 transition-colors ${
          isOver ? "bg-blue-primary/5" : ""
        }`}
      >
        <SortableContext items={column.cardIds} strategy={verticalListSortingStrategy}>
          <div className="space-y-2">
            {cards.map((card) => (
              <KanbanCard key={card.id} card={card} onDelete={onDeleteCard} />
            ))}
          </div>
        </SortableContext>

        <AddCardForm columnId={column.id} onAdd={onAddCard} />
      </div>
    </section>
  );
}
