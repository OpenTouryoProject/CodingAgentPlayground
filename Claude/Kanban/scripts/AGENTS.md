# Scripts

Start and stop the app in Docker. Run from anywhere; the scripts resolve the repo root.

- `start.sh` / `stop.sh` - Mac and Linux
- `start.ps1` / `stop.ps1` - Windows

`start` builds the image `pm-app`, removes any existing container, then runs a new one
mapping port 8000, passing the root `.env` via `--env-file`, and mounting the named volume
`pm-data` at the SQLite data dir so the board persists across container recreation. The app
is then at http://localhost:8000. `stop` removes the container (the `pm-data` volume is
kept).

Requires Docker installed and running.
