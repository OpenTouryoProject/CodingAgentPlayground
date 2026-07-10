"use client";

import { useState } from "react";

type AddCardFormProps = {
  columnId: string;
  onAdd: (columnId: string, title: string, details: string) => void;
};

export function AddCardForm({ columnId, onAdd }: AddCardFormProps) {
  const [isOpen, setIsOpen] = useState(false);
  const [title, setTitle] = useState("");
  const [details, setDetails] = useState("");

  function handleSubmit(event: React.FormEvent) {
    event.preventDefault();
    if (!title.trim()) return;

    onAdd(columnId, title, details);
    setTitle("");
    setDetails("");
    setIsOpen(false);
  }

  if (!isOpen) {
    return (
      <button
        type="button"
        onClick={() => setIsOpen(true)}
        className="mt-3 w-full rounded-lg border border-dashed border-blue-primary/30 px-3 py-2 text-sm font-medium text-blue-primary transition-colors hover:border-blue-primary hover:bg-blue-primary/5"
        data-testid={`add-card-toggle-${columnId}`}
      >
        + Add card
      </button>
    );
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="mt-3 space-y-2 rounded-lg border border-blue-primary/20 bg-white p-3 shadow-sm"
      data-testid={`add-card-form-${columnId}`}
    >
      <input
        type="text"
        value={title}
        onChange={(event) => setTitle(event.target.value)}
        placeholder="Card title"
        className="w-full rounded-md border border-gray-200 px-3 py-2 text-sm text-navy-dark outline-none focus:border-blue-primary focus:ring-1 focus:ring-blue-primary"
        data-testid={`add-card-title-${columnId}`}
        autoFocus
      />
      <textarea
        value={details}
        onChange={(event) => setDetails(event.target.value)}
        placeholder="Details (optional)"
        rows={2}
        className="w-full resize-none rounded-md border border-gray-200 px-3 py-2 text-sm text-gray-text outline-none focus:border-blue-primary focus:ring-1 focus:ring-blue-primary"
        data-testid={`add-card-details-${columnId}`}
      />
      <div className="flex gap-2">
        <button
          type="submit"
          className="rounded-md bg-purple-secondary px-3 py-1.5 text-sm font-medium text-white transition-colors hover:bg-purple-secondary/90"
          data-testid={`add-card-submit-${columnId}`}
        >
          Add
        </button>
        <button
          type="button"
          onClick={() => {
            setIsOpen(false);
            setTitle("");
            setDetails("");
          }}
          className="rounded-md px-3 py-1.5 text-sm text-gray-text transition-colors hover:text-navy-dark"
        >
          Cancel
        </button>
      </div>
    </form>
  );
}
