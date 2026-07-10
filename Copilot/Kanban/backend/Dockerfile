# Stage 1: build the static frontend
FROM node:24-slim AS frontend
WORKDIR /frontend
COPY frontend/package.json frontend/package-lock.json ./
RUN npm ci --no-audit --no-fund --fetch-retries=5 --fetch-retry-maxtimeout=120000
COPY frontend/ ./
RUN npm run build

# Stage 2: backend serving the built frontend
FROM python:3.14-slim
COPY --from=ghcr.io/astral-sh/uv:latest /uv /usr/local/bin/uv
WORKDIR /app
COPY backend/pyproject.toml backend/uv.lock* ./
RUN uv sync --no-dev
COPY backend/ ./
COPY --from=frontend /frontend/out/ ./app/static/
EXPOSE 8000
CMD ["uv", "run", "--no-dev", "uvicorn", "app.main:app", "--host", "0.0.0.0", "--port", "8000"]
