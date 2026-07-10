"use client";

import { useSortable } from "@dnd-kit/sortable";
import { CSS } from "@dnd-kit/utilities";
import type { Card } from "@/lib/types";

type KanbanCardProps = {
  card: Card;
  onDelete: (cardId: string) => void;
  isDragOverlay?: boolean;
};

export function KanbanCard({ card, onDelete, isDragOverlay = false }: KanbanCardProps) {
  const { attributes, listeners, setNodeRef, transform, transition, isDragging } =
    useSortable({
      id: card.id,
      disabled: isDragOverlay,
    });

  const style = isDragOverlay
    ? undefined
    : {
        transform: CSS.Transform.toString(transform),
        transition,
      };

  return (
    <div
      ref={isDragOverlay ? undefined : setNodeRef}
      style={style}
      {...(isDragOverlay ? {} : { ...attributes, ...listeners })}
      className={`group relative cursor-grab rounded-lg border border-gray-100 bg-white p-3 shadow-sm transition-shadow active:cursor-grabbing ${
        isDragOverlay
          ? "rotate-2 border-accent-yellow shadow-lg ring-2 ring-accent-yellow/40"
          : isDragging
            ? "opacity-40"
            : "hover:border-accent-yellow/40 hover:shadow-md"
      }`}
      data-testid={`card-${card.id}`}
    >
      <button
        type="button"
        onClick={(event) => {
          event.stopPropagation();
          onDelete(card.id);
        }}
        onPointerDown={(event) => event.stopPropagation()}
        className="absolute right-2 top-2 rounded p-1 text-gray-text opacity-0 transition-opacity hover:bg-red-50 hover:text-red-600 group-hover:opacity-100"
        aria-label={`Delete ${card.title}`}
        data-testid={`delete-card-${card.id}`}
      >
        <svg
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 20 20"
          fill="currentColor"
          className="h-4 w-4"
        >
          <path
            fillRule="evenodd"
            d="M8.75 1A2.75 2.75 0 006 3.75v.443c-.795.077-1.584.176-2.365.298a.75.75 0 10.23 1.482l.149-.022.841 10.518A2.75 2.75 0 007.596 19h4.807a2.75 2.75 0 002.742-2.53l.841-10.52.149.023a.75.75 0 00.23-1.482A41.03 41.03 0 0014 4.193V3.75A2.75 2.75 0 0011.25 1h-2.5zM10 4c.84 0 1.673.025 2.5.075V3.75c0-.69-.56-1.25-1.25-1.25h-2.5c-.69 0-1.25.56-1.25 1.25v.325C8.327 4.025 9.16 4 10 4zM8.58 7.72a.75.75 0 00-1.5.06l.3 7.5a.75.75 0 101.5-.06l-.3-7.5zm4.34.06a.75.75 0 10-1.5-.06l-.3 7.5a.75.75 0 101.5.06l.3-7.5z"
            clipRule="evenodd"
          />
        </svg>
      </button>
      <h3 className="pr-6 text-sm font-semibold text-navy-dark">{card.title}</h3>
      {card.details ? (
        <p className="mt-1 text-sm leading-relaxed text-gray-text">{card.details}</p>
      ) : null}
    </div>
  );
}

export function KanbanCardPreview({ card }: { card: Card }) {
  return (
    <div className="rotate-2 rounded-lg border border-accent-yellow bg-white p-3 shadow-lg ring-2 ring-accent-yellow/40">
      <h3 className="text-sm font-semibold text-navy-dark">{card.title}</h3>
      {card.details ? (
        <p className="mt-1 text-sm leading-relaxed text-gray-text">{card.details}</p>
      ) : null}
    </div>
  );
}
