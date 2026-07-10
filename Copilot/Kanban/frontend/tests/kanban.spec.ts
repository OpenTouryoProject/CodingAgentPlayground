import { expect, test, type Page } from "@playwright/test";

type Card = { id: string; title: string; details: string };
type Column = { id: string; title: string; cardIds: string[] };
type BoardData = { columns: Column[]; cards: Record<string, Card> };

const seed = (): BoardData => ({
  columns: [
    { id: "col-backlog", title: "Backlog", cardIds: ["card-1", "card-2"] },
    { id: "col-discovery", title: "Discovery", cardIds: ["card-3"] },
    { id: "col-progress", title: "In Progress", cardIds: ["card-4", "card-5"] },
    { id: "col-review", title: "Review", cardIds: ["card-6"] },
    { id: "col-done", title: "Done", cardIds: ["card-7", "card-8"] },
  ],
  cards: {
    "card-1": { id: "card-1", title: "Align roadmap themes", details: "Themes." },
    "card-2": { id: "card-2", title: "Gather customer signals", details: "Signals." },
    "card-3": { id: "card-3", title: "Prototype analytics view", details: "Sketch." },
    "card-4": { id: "card-4", title: "Refine status language", details: "Labels." },
    "card-5": { id: "card-5", title: "Design card layout", details: "Spacing." },
    "card-6": { id: "card-6", title: "QA micro-interactions", details: "States." },
    "card-7": { id: "card-7", title: "Ship marketing page", details: "Copy." },
    "card-8": { id: "card-8", title: "Close onboarding sprint", details: "Notes." },
  },
});

const installBoardApi = async (page: Page) => {
  const board = seed();
  let counter = 0;

  await page.route("**/api/board", (route) =>
    route.fulfill({ json: board })
  );

  await page.route("**/api/columns/*", (route) => {
    const columnId = route.request().url().split("/api/columns/")[1];
    const { title } = route.request().postDataJSON();
    const column = board.columns.find((item) => item.id === columnId);
    if (column) {
      column.title = title;
    }
    return route.fulfill({ json: board });
  });

  await page.route("**/api/cards/*/move", (route) => {
    const cardId = route.request().url().split("/api/cards/")[1].replace("/move", "");
    const { columnId, position } = route.request().postDataJSON();
    for (const column of board.columns) {
      column.cardIds = column.cardIds.filter((id) => id !== cardId);
    }
    const target = board.columns.find((item) => item.id === columnId);
    if (target) {
      target.cardIds.splice(position, 0, cardId);
    }
    return route.fulfill({ json: board });
  });

  await page.route("**/api/cards/*", (route) => {
    const method = route.request().method();
    const cardId = route.request().url().split("/api/cards/")[1];
    if (method === "PATCH") {
      const { title, details } = route.request().postDataJSON();
      board.cards[cardId] = { id: cardId, title, details };
    } else if (method === "DELETE") {
      delete board.cards[cardId];
      for (const column of board.columns) {
        column.cardIds = column.cardIds.filter((id) => id !== cardId);
      }
    }
    return route.fulfill({ json: board });
  });

  await page.route("**/api/ai/chat", (route) => {
    for (const column of board.columns) {
      column.cardIds = column.cardIds.filter((id) => id !== "card-1");
    }
    const done = board.columns.find((item) => item.id === "col-done");
    if (done) {
      done.cardIds.push("card-1");
    }
    return route.fulfill({ json: { reply: "Moved it to Done.", board } });
  });

  await page.route("**/api/cards", (route) => {
    const { columnId, title, details } = route.request().postDataJSON();
    const id = `card-e2e-${counter++}`;
    board.cards[id] = { id, title, details };
    const column = board.columns.find((item) => item.id === columnId);
    if (column) {
      column.cardIds.push(id);
    }
    return route.fulfill({ json: board });
  });
};

const mockAuthenticated = async (page: Page, authenticated: boolean) => {
  await page.route("**/api/me", (route) =>
    route.fulfill({ json: { authenticated } })
  );
};

test.describe("authenticated board", () => {
  test.beforeEach(async ({ page }) => {
    await mockAuthenticated(page, true);
    await installBoardApi(page);
  });

  test("loads the kanban board", async ({ page }) => {
    await page.goto("/");
    await expect(
      page.getByRole("heading", { name: "Kanban Studio" })
    ).toBeVisible();
    await expect(page.locator('[data-testid^="column-"]')).toHaveCount(5);
  });

  test("adds a card and persists across reload", async ({ page }) => {
    await page.goto("/");
    const firstColumn = page.locator('[data-testid^="column-"]').first();
    await firstColumn.getByRole("button", { name: /add a card/i }).click();
    await firstColumn.getByPlaceholder("Card title").fill("Playwright card");
    await firstColumn.getByPlaceholder("Details").fill("Added via e2e.");
    await firstColumn.getByRole("button", { name: /add card/i }).click();
    await expect(firstColumn.getByText("Playwright card")).toBeVisible();

    await page.reload();
    await expect(page.getByText("Playwright card")).toBeVisible();
  });

  test("moves a card between columns and persists", async ({ page }) => {
    await page.goto("/");
    const card = page.getByTestId("card-card-1");
    const targetColumn = page.getByTestId("column-col-review");
    const cardBox = await card.boundingBox();
    const columnBox = await targetColumn.boundingBox();
    if (!cardBox || !columnBox) {
      throw new Error("Unable to resolve drag coordinates.");
    }

    await page.mouse.move(
      cardBox.x + cardBox.width / 2,
      cardBox.y + cardBox.height / 2
    );
    await page.mouse.down();
    await page.mouse.move(
      columnBox.x + columnBox.width / 2,
      columnBox.y + 120,
      { steps: 12 }
    );
    await page.mouse.up();
    await expect(targetColumn.getByTestId("card-card-1")).toBeVisible();

    await page.reload();
    await expect(
      page.getByTestId("column-col-review").getByTestId("card-card-1")
    ).toBeVisible();
  });

  test("edits a card and persists across reload", async ({ page }) => {
    await page.goto("/");
    const card = page.getByTestId("card-card-1");
    await card
      .getByRole("button", { name: /edit align roadmap themes/i })
      .click();
    await card.getByLabel("Card title").fill("Edited via e2e");
    await card.getByLabel("Card details").fill("New details via e2e");
    await card.getByRole("button", { name: /save/i }).click();
    await expect(page.getByText("Edited via e2e")).toBeVisible();

    await page.reload();
    await expect(page.getByText("Edited via e2e")).toBeVisible();
    await expect(page.getByText("New details via e2e")).toBeVisible();
  });

  test("updates the board from the AI chat without a reload", async ({ page }) => {
    await page.goto("/");
    await page.getByLabel("Message").fill("Move card-1 to Done");
    await page.getByRole("button", { name: /send/i }).click();

    await expect(page.getByText("Moved it to Done.")).toBeVisible();
    await expect(
      page.getByTestId("column-col-done").getByTestId("card-card-1")
    ).toBeVisible();
  });
});

test("requires login and supports logout", async ({ page }) => {
  let authenticated = false;
  await page.route("**/api/me", (route) =>
    route.fulfill({ json: { authenticated } })
  );
  await page.route("**/api/login", (route) => {
    authenticated = true;
    return route.fulfill({ json: { ok: true } });
  });
  await page.route("**/api/logout", (route) => {
    authenticated = false;
    return route.fulfill({ json: { ok: true } });
  });
  await installBoardApi(page);

  await page.goto("/");
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();

  await page.getByLabel("Username").fill("user");
  await page.getByLabel("Password").fill("password");
  await page.getByRole("button", { name: /sign in/i }).click();

  await expect(
    page.getByRole("heading", { name: "Kanban Studio" })
  ).toBeVisible();

  await page.getByRole("button", { name: /log out/i }).click();
  await expect(page.getByRole("heading", { name: "Sign in" })).toBeVisible();
});
