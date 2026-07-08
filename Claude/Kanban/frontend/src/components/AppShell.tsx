"use client";

import { useEffect, useState } from "react";
import { BoardView } from "@/components/BoardView";
import { LoginForm } from "@/components/LoginForm";
import { getMe, login, logout } from "@/lib/api";

type AuthState = "loading" | "signed-out" | "signed-in";

export const AppShell = () => {
  const [status, setStatus] = useState<AuthState>("loading");
  const [user, setUser] = useState<string | null>(null);

  useEffect(() => {
    getMe().then((me) => {
      setUser(me);
      setStatus(me ? "signed-in" : "signed-out");
    });
  }, []);

  const handleLogin = async (username: string, password: string) => {
    await login(username, password);
    const me = await getMe();
    setUser(me);
    setStatus(me ? "signed-in" : "signed-out");
  };

  const handleLogout = async () => {
    await logout();
    setUser(null);
    setStatus("signed-out");
  };

  if (status === "loading") {
    return (
      <main className="grid min-h-screen place-items-center">
        <p className="text-sm font-semibold uppercase tracking-[0.3em] text-[var(--gray-text)]">
          Loading...
        </p>
      </main>
    );
  }

  if (status === "signed-out") {
    return <LoginForm onLogin={handleLogin} />;
  }

  return (
    <div>
      <div className="flex items-center justify-end gap-4 px-6 pt-6">
        <span className="text-xs font-semibold uppercase tracking-[0.2em] text-[var(--gray-text)]">
          Signed in as {user}
        </span>
        <button
          type="button"
          onClick={handleLogout}
          className="rounded-full border border-[var(--stroke)] px-4 py-2 text-xs font-semibold uppercase tracking-wide text-[var(--navy-dark)] transition hover:border-[var(--primary-blue)] hover:text-[var(--primary-blue)]"
        >
          Log out
        </button>
      </div>
      <BoardView />
    </div>
  );
};
