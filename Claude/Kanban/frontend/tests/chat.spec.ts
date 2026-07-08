import { expect, test } from "@playwright/test";
import { login } from "./helpers";

// This test exercises a live OpenRouter call via the backend, so give it room.
test.beforeEach(async ({ page }) => {
  await login(page);
});

test("assistant adds a card and the board auto-refreshes", async ({ page }) => {
  test.setTimeout(90_000);
  const title = `AI card ${Date.now()}`;

  await page.getByRole("button", { name: /open assistant/i }).click();
  await page
    .getByLabel(/message the assistant/i)
    .fill(`Add a new card titled "${title}" to the Backlog column. Keep details short.`);
  await page.getByRole("button", { name: /send/i }).click();

  // An assistant reply bubble appears.
  await expect(page.getByText("Thinking...")).toBeHidden({ timeout: 60_000 });

  // The board refreshes automatically (no manual reload): the new card shows in Backlog.
  await expect(page.getByTestId("column-col-backlog")).toContainText(title, {
    timeout: 60_000,
  });
});
