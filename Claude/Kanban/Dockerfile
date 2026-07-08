# Multi-stage build: build the frontend static export, then serve it from the
# FastAPI backend. Build context is the repo root.

# Stage 1: build the Next.js static export -> frontend/out
FROM node:22-slim AS frontend
WORKDIR /app/frontend
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci
COPY frontend/ ./
RUN npm run build

# Stage 2: backend runtime + static site
FROM python:3.12-slim

# uv binary
COPY --from=ghcr.io/astral-sh/uv:latest /uv /uvx /bin/

WORKDIR /app/backend

# Install dependencies (no dev deps in the image)
COPY backend/pyproject.toml backend/uv.lock ./
RUN uv sync --no-dev --frozen

# App code
COPY backend/ ./

# Place the exported frontend where the backend serves it (at /)
COPY --from=frontend /app/frontend/out ./app/static

EXPOSE 8000

CMD ["uv", "run", "--no-dev", "uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8000"]
