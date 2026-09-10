---
description: Chained workflow to add a status audit trail — Graphify analysis, StatusHistory entity + migration, transition validation, history endpoint, review checklist
---

# /backend-audit

Chained, ordered workflow for adding an authorization status audit trail to the backend. Run the steps below **in sequence** — later steps depend on earlier ones. Do not reorder, skip, or parallelize steps unless a step itself says to.

## Steps

1. **Run Graphify analysis.**
   Invoke the `graphify` skill against the repo root (build the graph if `graphify-out/graph.json` doesn't exist yet). Query it for callers of `AuthorizationsController.UpdateStatus` and `graphify explain "Authorization"` to confirm the blast radius of adding status history before touching any code.

2. **Create `StatusHistory` entity.**
   Add a new EF Core entity in `backend/PriorAuth.API/Models/Entities.cs`: authorization id (FK to `Authorization`), previous status, new status, changed-by, changed-at. Follow existing conventions — snake_case DB columns via explicit `[Column("...")]` attributes, `PascalCase` C# properties, no separate entity file (per `CLAUDE.md`).

3. **Create EF migration.**
   Generate the migration for the new table (`dotnet ef migrations add AddStatusHistory` from `backend/PriorAuth.API`). Do not run `dotnet ef database update` / apply it without explicit confirmation.

4. **Launch a background subagent to implement status transition validation.**
   Use the `Agent` tool (not inline in the main thread) to implement validation of allowed status transitions inside `AuthorizationsController.UpdateStatus` (e.g. `PENDING` → `IN_REVIEW` → `APPROVED`/`DENIED`; `CANCELLED` reachable from any non-terminal state), rejecting invalid transitions with a 400 and writing a `StatusHistory` row on each accepted transition.

5. **Use a skill for transition validation.**
   Before merging the subagent's output, check whether this repo has a dedicated validation/compliance skill (e.g. a workshop-added `/spec-review` or `/hipaa-check` per `CLAUDE.md`'s "Intentionally incomplete" section) and run it against the new transition logic. If no such skill exists yet in this repo, fall back to the `code-review` skill scoped to the transition-validation diff — do not invent a skill name.

6. **Add `GET /api/authorizations/{id}/history`.**
   New endpoint on `AuthorizationsController` returning `StatusHistory` rows for the given authorization, via a new DTO in `Dtos.cs`, mirrored into the frontend's `types/index.ts` and wired through `api/client.ts`.

7. **Review implementation and generate checklist.**
   Run the `code-review` skill over the full diff from steps 2–6. Produce a markdown checklist (done vs. outstanding: entity/migration correctness, transition rules, endpoint + DTO + frontend mirror, manual verification) rather than prose.

## Constraints

- Steps 1, 5, 7 are analysis/review only — no source edits.
- Steps 2, 3, 6 touch backend code (and step 6 touches frontend types/client).
- Never apply the migration to the database without explicit user confirmation.
- Match all existing conventions in `CLAUDE.md` (status strings, DTO record style, `MapToDetail` reuse, snake_case↔PascalCase column mapping).
