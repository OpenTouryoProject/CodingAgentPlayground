"use client";

import { useEffect, useState } from "react";
import { KanbanBoard } from "@/components/KanbanBoard";
import { LoginForm } from "@/components/LoginForm";

export const AppShell = () => {
  const [authenticated, setAuthenticated] = useState<boolean | null>(null);

  useEffect(() => {
    fetch("/api/me")
      .then((response) => response.json())
      .then((data) => setAuthenticated(data.authenticated))
      .catch(() => setAuthenticated(false));
  }, []);

  const handleLogout = async () => {
    await fetch("/api/logout", { method: "POST" });
    setAuthenticated(false);
  };

  if (authenticated === null) {
    return (
      <div className="flex min-h-screen items-center justify-center text-sm text-[var(--gray-text)]">
        Loading...
      </div>
    );
  }

  if (!authenticated) {
    return <LoginForm onSuccess={() => setAuthenticated(true)} />;
  }

  return <KanbanBoard onLogout={handleLogout} />;
};
