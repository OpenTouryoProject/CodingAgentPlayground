import { expect, test } from "@playwright/test";

test.describe("Kanban board", () => {
  test("loads dummy data", async ({ page }) => {
    await page.goto("/");

    await expect(page.getByRole("heading", { name: "Project Board" })).toBeVisible();
    await expect(page.getByTestId("column-title-col-backlog")).toHaveText("Backlog");
    await expect(page.getByTestId("column-title-col-done")).toHaveText("Done");
    await expect(page.getByText("Research competitors")).toBeVisible();
  });

  test("renames a column", async ({ page }) => {
    await page.goto("/");

    await page.getByTestId("column-title-col-backlog").click();
    const input = page.getByTestId("column-rename-input-col-backlog");
    await input.fill("Ideas");
    await input.press("Enter");

    await expect(page.getByTestId("column-title-col-backlog")).toHaveText("Ideas");
  });

  test("adds a card to a column", async ({ page }) => {
    await page.goto("/");

    await page.getByTestId("add-card-toggle-col-todo").click();
    await page.getByTestId("add-card-title-col-todo").fill("E2E test card");
    await page.getByTestId("add-card-details-col-todo").fill("Added by Playwright");
    await page.getByTestId("add-card-submit-col-todo").click();

    await expect(page.getByText("E2E test card")).toBeVisible();
    await expect(page.getByText("Added by Playwright")).toBeVisible();
  });

  test("deletes a card", async ({ page }) => {
    await page.goto("/");

    const card = page.getByTestId("card-card-10");
    await expect(card).toBeVisible();
    await page.getByTestId("delete-card-card-10").click({ force: true });

    await expect(page.getByText("Ship MVP")).not.toBeVisible();
  });

  test("drags a card between columns", async ({ page }) => {
    await page.goto("/");

    const card = page.getByTestId("card-card-1");
    const targetColumn = page.getByTestId("column-col-todo");

    await expect(card).toBeVisible();

    const cardBox = await card.boundingBox();
    const targetBox = await targetColumn.boundingBox();
    if (!cardBox || !targetBox) {
      throw new Error("Could not resolve drag targets");
    }

    const startX = cardBox.x + cardBox.width / 2;
    const startY = cardBox.y + cardBox.height / 2;
    const endX = targetBox.x + targetBox.width / 2;
    const endY = targetBox.y + 120;

    await page.mouse.move(startX, startY);
    await page.mouse.down();
    await page.mouse.move(startX + 10, startY, { steps: 5 });
    await page.mouse.move(endX, endY, { steps: 20 });
    await page.mouse.up();

    await expect(
      page.getByTestId("column-col-todo").getByText("Research competitors"),
    ).toBeVisible();
    await expect(
      page.getByTestId("column-col-backlog").getByText("Research competitors"),
    ).not.toBeVisible();
  });
});
