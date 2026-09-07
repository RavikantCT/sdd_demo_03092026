---
name: status-transition-validator
description: Define legal authorization status transitions, reject illegal ones in AuthorizationsController.UpdateStatus, and generate tests covering valid and invalid transitions. Use when adding or reviewing status-change logic for prior authorizations.
---

# /status-transition-validator

Validates and tests status transitions for the `Authorization` resource. Scope is exactly the three responsibilities below — it does not persist audit rows, add endpoints, or touch the frontend; those belong to the calling workflow (e.g. `/backend-audit`).

## Context

The five legal status values (`CLAUDE.md`, `frontend/src/types/index.ts` `STATUS_COLORS`/`STATUS_BG`) are: `PENDING`, `APPROVED`, `DENIED`, `CANCELLED`, `IN_REVIEW`.

Today `AuthorizationsController.UpdateStatus` (`backend/PriorAuth.API/Controllers/AuthorizationsController.cs:141-156`) only checks that the *target* status is one of these five — it does not check that the transition from the record's *current* status is legal. E.g. `APPROVED → PENDING` currently succeeds. That is the gap this skill closes.

## 1. Define legal transitions

No transition rules exist anywhere in the codebase today — this state machine is a proposed default, not something recovered from existing code. Confirm it with the user before relying on it for anything beyond a first draft:

```
PENDING    -> IN_REVIEW, CANCELLED
IN_REVIEW  -> APPROVED, DENIED, CANCELLED
APPROVED   -> (terminal — no transitions out)
DENIED     -> (terminal — no transitions out)
CANCELLED  -> (terminal — no transitions out)
```

Rationale: `APPROVED`/`DENIED`/`CANCELLED` are terminal outcomes; `CANCELLED` is reachable from either non-terminal state (a request can be withdrawn before or during review); a request must pass through `IN_REVIEW` before a decision. Same-status "transitions" (e.g. `PENDING -> PENDING`) are illegal (no-op) — reject them like any other non-adjacent transition.

Represent this as a single lookup table (`Dictionary<string, string[]>` keyed by current status) next to `validStatuses` in `AuthorizationsController.cs` — do not scatter the rule across multiple methods.

## 2. Reject illegal transitions

In `UpdateStatus`, after the existing "is this a known status" check, add a guard: look up `auth.Status` (current) in the transitions table and confirm `request.Status.ToUpper()` is in its allowed-next-states list. On failure, return `400 BadRequest` with a message naming both the current status and the allowed next states (mirror the existing error-message style at line 149), e.g.:

```
Cannot transition from APPROVED to PENDING. Valid next states from APPROVED: none (terminal).
```

Do not throw for an already-terminal status silently — the 400 message should make the terminal state legible to the API caller.

## 3. Generate tests for valid and invalid transitions

This repo has no test project today (`CLAUDE.md`: "There are no automated tests or linters configured"). Adding one is in scope for this skill's responsibility, since transition validation is meaningless unvalidated:

- Create `backend/PriorAuth.API.Tests/` as a new xUnit test project (`dotnet new xunit`), referencing `PriorAuth.API` and using EF Core's in-memory provider (or SQLite in-memory) for `PriorAuthDbContext` — do not hit the real Postgres instance from tests.
- Generate one test case per **legal** edge in the transition table (e.g. `PENDING -> IN_REVIEW` returns success, status persisted).
- Generate one test case per **illegal** edge, covering at minimum: reverse of every legal edge (e.g. `IN_REVIEW -> PENDING`), any transition out of each terminal state, and same-status no-ops — each asserting `400 BadRequest` and that `auth.Status`/`UpdatedAt` are unchanged.
- Also keep one test for an unrecognized status string (e.g. `"BOGUS"`) to confirm the pre-existing valid-status check still fires before the new transition check.
- Name tests descriptively: `UpdateStatus_<From>To<To>_<Succeeds|Rejects>`.

## Output

When invoked, produce (in this order): the transition table as a code diff/snippet, the updated `UpdateStatus` guard clause, and the generated test file(s) — plus a one-line note of any transition-table assumption the user should confirm.
