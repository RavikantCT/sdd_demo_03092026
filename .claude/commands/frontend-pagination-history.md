---
description: Add dashboard pagination + a History Panel, graphify-first and mock-contract-first
---

Implement dashboard pagination and a History Panel for the authorizations dashboard. $ARGUMENTS

Follow these steps in order. Do not skip or reorder them.

## 1. Graphify analysis first

Before any code changes, run a Graphify analysis of the areas this touches:

- `graphify path "DashboardPage" "AuthorizationsController.GetAll"`
- `graphify query "what calls api.authorizations.getAll"`
- `graphify query "what renders the authorizations list"`

If `graphify-out/graph.json` doesn't exist yet or is stale (files changed since last build), run the full `/graphify` pipeline first, then the queries above. Report findings only — no code changes yet.

## 2. Implement dashboard pagination

In `frontend/src/pages/DashboardPage.tsx`, add pagination to the authorizations list (page/pageSize controls, current-page state, next/prev or page-number navigation). Only touch what pagination requires — do not refactor unrelated parts of the page.

If the backend needs to support paging (`AuthorizationsController.GetAll` in `backend/PriorAuth.API/Controllers/AuthorizationsController.cs`), add `page`/`pageSize` query params and a paged response shape there too. Mirror any DTO shape change into `frontend/src/types/index.ts` per this repo's convention (DB → EF entity → DTO → TS type, all four layers by hand).

## 3. Launch a background subagent for the History Panel UI

Dispatch a fresh subagent (not a fork — this is independent, parallel UI work) to build the History Panel component. Give it full context: what the panel shows (an authorization's status-change history), where it plugs in (likely `AuthorizationDetailPage.tsx` or a new component under `frontend/src/components/`), and the mocked contract from step 4. Let it run in the background while you continue with pagination.

## 4. Build against a mocked response contract initially

Before wiring either feature to a real backend endpoint, agree on and hardcode a mock response shape:

- Pagination: `{ items: AuthorizationSummary[], page: number, pageSize: number, totalCount: number }`
- History panel: an array of history entries (status, timestamp, actor) — shape decided when the subagent starts, consistent with existing DTO naming conventions (camelCase, matches `AuthorizationSummaryDto`-style fields)

Both pagination and the history panel should render correctly against the mock before either is wired to a live endpoint.

## 5. Reuse STATUS_COLORS and STATUS_BG

Both the paginated list and the History Panel must use the existing `STATUS_COLORS`/`STATUS_BG` maps from `frontend/src/types/index.ts` for status display — do not redefine new color maps. If the History Panel needs a status not already in those maps, stop and ask rather than inventing a new color silently (status values are fixed uppercase strings validated in `AuthorizationsController.UpdateStatus` — all three places must stay in sync).

## 6. Review implementation

Once pagination and the History Panel are both in place (and the background subagent has finished), review the combined implementation — via `/code-review` or a manual pass — for correctness, consistency between the two features, and that the mocked contract was fully replaced with the real one (no leftover mock data paths).
