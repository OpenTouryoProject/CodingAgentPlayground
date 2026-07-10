"use client";

import { useState, type FormEvent } from "react";
import type { BoardData } from "@/lib/kanban";
import { chatApi, type ChatMessage } from "@/lib/api";

type ChatSidebarProps = {
  onBoardUpdate: (board: BoardData) => void;
};

export const ChatSidebar = ({ onBoardUpdate }: ChatSidebarProps) => {
  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [input, setInput] = useState("");
  const [pending, setPending] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    const question = input.trim();
    if (!question || pending) {
      return;
    }

    const history = messages;
    setMessages([...history, { role: "user", content: question }]);
    setInput("");
    setPending(true);
    setError(null);

    try {
      const data = await chatApi(question, history);
      setMessages((prev) => [...prev, { role: "assistant", content: data.reply }]);
      onBoardUpdate(data.board);
    } catch {
      setError("The assistant is unavailable right now.");
    } finally {
      setPending(false);
    }
  };

  return (
    <section
      className="flex h-full min-h-[520px] flex-col rounded-[28px] border border-[var(--stroke)] bg-white/80 p-5 shadow-[var(--shadow)] backdrop-blur"
      aria-label="AI assistant"
    >
      <div className="flex items-center gap-3 border-b border-[var(--stroke)] pb-4">
        <span className="h-2 w-10 rounded-full bg-[var(--secondary-purple)]" />
        <div>
          <h2 className="font-display text-lg font-semibold text-[var(--navy-dark)]">
            Assistant
          </h2>
          <p className="text-xs font-semibold uppercase tracking-[0.2em] text-[var(--gray-text)]">
            Ask to update the board
          </p>
        </div>
      </div>

      <div
        className="mt-4 flex flex-1 flex-col gap-3 overflow-y-auto"
        data-testid="chat-messages"
      >
        {messages.length === 0 && (
          <p className="text-sm leading-6 text-[var(--gray-text)]">
            Try &ldquo;Move QA micro-interactions to Done&rdquo; or &ldquo;Add a
            card to Backlog for the launch checklist.&rdquo;
          </p>
        )}
        {messages.map((message, index) => (
          <div
            key={index}
            className={
              message.role === "user"
                ? "self-end rounded-2xl rounded-br-sm bg-[var(--primary-blue)] px-4 py-2 text-sm text-white"
                : "self-start rounded-2xl rounded-bl-sm border border-[var(--stroke)] bg-[var(--surface)] px-4 py-2 text-sm text-[var(--navy-dark)]"
            }
            data-role={message.role}
          >
            {message.content}
          </div>
        ))}
        {pending && (
          <div className="self-start text-xs font-semibold uppercase tracking-[0.2em] text-[var(--gray-text)]">
            Thinking...
          </div>
        )}
      </div>

      {error && (
        <p className="mt-3 text-sm font-medium text-[#c0392b]" role="alert">
          {error}
        </p>
      )}

      <form onSubmit={handleSubmit} className="mt-4 flex items-center gap-2">
        <input
          value={input}
          onChange={(event) => setInput(event.target.value)}
          placeholder="Ask the assistant"
          aria-label="Message"
          className="flex-1 rounded-full border border-[var(--stroke)] bg-white px-4 py-2 text-sm text-[var(--navy-dark)] outline-none transition focus:border-[var(--primary-blue)]"
        />
        <button
          type="submit"
          disabled={pending}
          className="rounded-full bg-[var(--secondary-purple)] px-4 py-2 text-sm font-semibold uppercase tracking-wide text-white transition hover:brightness-110 disabled:opacity-50"
        >
          Send
        </button>
      </form>
    </section>
  );
};
