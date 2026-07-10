"use client";

import { useState, type FormEvent } from "react";

type LoginFormProps = {
  onSuccess: () => void;
};

export const LoginForm = ({ onSuccess }: LoginFormProps) => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setError("");
    const response = await fetch("/api/login", {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ username, password }),
    });
    if (response.ok) {
      onSuccess();
    } else {
      setError("Invalid username or password.");
    }
  };

  return (
    <div className="flex min-h-screen items-center justify-center px-6">
      <form
        onSubmit={handleSubmit}
        className="w-full max-w-sm rounded-[28px] border border-[var(--stroke)] bg-white/80 p-8 shadow-[var(--shadow)] backdrop-blur"
      >
        <h1 className="font-display text-3xl font-semibold text-[var(--navy-dark)]">
          Sign in
        </h1>
        <p className="mt-2 text-sm text-[var(--gray-text)]">
          Sign in to view your board.
        </p>
        <label className="mt-6 block text-xs font-semibold uppercase tracking-wide text-[var(--gray-text)]">
          Username
          <input
            value={username}
            onChange={(event) => setUsername(event.target.value)}
            className="mt-2 w-full rounded-xl border border-[var(--stroke)] bg-white px-3 py-2 text-sm font-medium text-[var(--navy-dark)] outline-none transition focus:border-[var(--primary-blue)]"
            required
          />
        </label>
        <label className="mt-4 block text-xs font-semibold uppercase tracking-wide text-[var(--gray-text)]">
          Password
          <input
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            className="mt-2 w-full rounded-xl border border-[var(--stroke)] bg-white px-3 py-2 text-sm font-medium text-[var(--navy-dark)] outline-none transition focus:border-[var(--primary-blue)]"
            required
          />
        </label>
        {error && (
          <p className="mt-4 text-sm font-medium text-[#c0392b]" role="alert">
            {error}
          </p>
        )}
        <button
          type="submit"
          className="mt-6 w-full rounded-full bg-[var(--secondary-purple)] px-4 py-2 text-sm font-semibold uppercase tracking-wide text-white transition hover:brightness-110"
        >
          Sign in
        </button>
      </form>
    </div>
  );
};
