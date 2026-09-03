# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

ABC Healthcare Prior Authorization intake system — a demo app used as the baseline for an SDD (Spec-Driven Development) + Claude Code CLI workshop. React (TypeScript) frontend, ASP.NET Core 8 Web API backend, PostgreSQL 16 database (Docker).

**Intentionally incomplete:** Member Eligibility verification does not exist yet. It is the feature the workshop adds via spec-driven development (a `SPEC.md` defining the feature, plus custom skills like `/spec-review` and `/hipaa-check` to verify generation steps against it). If such files appear later in the repo, treat them as authoritative for that feature's requirements.

## Running the stack

Three independent processes, no single "run everything" script:

```bash
# 1. Database (Docker)
docker compose up -d
docker compose ps          # wait for pa_db to show "healthy"

# 2. Backend API — http://localhost:5000, Swagger at /swagger
cd backend/PriorAuth.API
dotnet run

# 3. Frontend — http://localhost:5173 (Vite dev server proxies /api -> :5000)
cd frontend
npm install
npm run dev
```

Reset the DB (drops all data and re-runs `database/init.sql` seed):
```bash
docker compose down -v
docker compose up -d
```

There are no automated tests or linters configured in this repo (no test projects, no ESLint config). `npm run build` (which runs `tsc && vite build`) is the only frontend check available; `dotnet build` is the backend equivalent.

## Architecture

**Data flow:** PostgreSQL (`database/init.sql` — schema + seed data, snake_case columns) → EF Core entities (`backend/PriorAuth.API/Models/Entities.cs`, mapped via `[Column("...")]` attributes) → controllers project entities into `record` DTOs (`DTOs/Dtos.cs`, camelCase in JSON) → frontend TypeScript interfaces (`frontend/src/types/index.ts`) that mirror the DTOs field-for-field. When changing a table, the change must be threaded through all four layers by hand — there is no code generation.

**Backend** (`backend/PriorAuth.API/`) — minimal, no service/repository layer; controllers talk to `PriorAuthDbContext` directly:
- `Controllers/LookupControllers.cs` — one controller class per read-only master table: `MembersController`, `ProvidersController`, `SitesController`, `HealthPlansController`, `DiagnosisCodesController`, `ProcedureCodesController`. All follow the same search-with-`Take(N)`-limit pattern.
- `Controllers/AuthorizationsController.cs` — the one writable resource. `Create` generates a `PA-YYYYMMDD-XXXXX` reference number, inserts the `Authorization` row, then inserts child `AuthorizationProcedure`/`AuthorizationDiagnosis` rows in a second `SaveChangesAsync`. `MapToDetail` is the single place that assembles the full nested DTO graph — reuse it rather than duplicating projection logic.
- `Program.cs` — CORS is locked to `http://localhost:5173`/`:3000` (`ReactDev` policy); update this if the frontend origin changes.
- Connection string lives in `appsettings.json` (`ConnectionStrings:DefaultConnection`). Note it does not currently match the Postgres user/password/db defined in `docker-compose.yml` — check both when diagnosing local connection failures.

**Frontend** (`frontend/src/`) — no state management library; each page owns its own state:
- `api/client.ts` — the only place `fetch` is called; every backend call goes through the `api` object here.
- `types/index.ts` — hand-mirrors the backend DTOs, plus frontend-only shapes (`WizardState`, `PROGRAMS`, `STATUS_COLORS`/`STATUS_BG`).
- `components/SearchSelect.tsx` — reusable typeahead used by every lookup (member, provider, site, diagnosis, procedure) search field.
- `pages/NewAuthorizationPage.tsx` — the 6-step new-request wizard (Program → Provider → Health Plan → Member → Diagnosis & CPT → Site & Review), accumulating into a single `WizardState` before calling `api.authorizations.create`.
- `pages/DashboardPage.tsx` / `AuthorizationDetailPage.tsx` — list and detail views for existing authorizations.
- Routing is plain `react-router-dom` (`App.tsx`): `/`, `/new`, `/authorization/:id`.

## Conventions to preserve

- DB columns are `snake_case`; EF entity properties are `PascalCase` via explicit `[Column(...)]` attributes — keep this mapping explicit rather than relying on naming convention configuration.
- DTOs are C# `record` types, one block per concern (lookup DTOs vs. authorization DTOs) in a single `Dtos.cs` file — don't split into per-entity files.
- Status values are fixed uppercase strings (`PENDING`, `APPROVED`, `DENIED`, `CANCELLED`, `IN_REVIEW`), validated in `AuthorizationsController.UpdateStatus` and mirrored in the frontend's `STATUS_COLORS`/`STATUS_BG` maps — update all three together if adding a status.
