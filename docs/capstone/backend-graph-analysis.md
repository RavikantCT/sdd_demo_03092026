# Backend Graph Analysis (Graphify)

Analysis generated via `graphify extract . --code-only` on the repository root prior to any code changes on `feature/status-audit-trail`.

## Graph summary

- 260 nodes, 387 edges, 13 communities
- 20 code files indexed (backend C# + frontend TS)
- `.sql` files were not extracted (`tree_sitter_sql` dependency not installed) — `database/init.sql` schema is not represented in this graph, only code-level (C#/TS) relationships

## Query: "what calls UpdateStatus in AuthorizationsController"

No caller edges were found into `AuthorizationsController.UpdateStatus()` (`backend/PriorAuth.API/Controllers/AuthorizationsController.cs:L141`) — only the method node and its containing class matched, with no incoming `call` edges.

This is expected: `UpdateStatus` is an ASP.NET Core controller action, invoked by the routing framework at runtime (HTTP `PATCH`/`PUT`) rather than called directly by other code in the repo, so static AST extraction finds no in-repo caller.

## Explain: "Authorization"

`Authorization` entity — `backend/PriorAuth.API/Models/Entities.cs:L207`, degree 31, community 0. The graph's central data model.

- Referenced by `PriorAuthDbContext` (DbSet) and its `OnModelCreating()`
- Defines child navigation properties: `Provider`, `Site`, `Member`, `HealthPlan`, `AuthorizationDiagnosis`, `AuthorizationProcedure`, plus scalar fields (`AuthorizationId`, `ReferenceNumber`, `Status`, `Program`, `CreatedAt`, `UpdatedAt`, ...)
- Called into by `AuthorizationsController.Create()` (L82) and `MapToDetail()` (L158) — confirms `MapToDetail` as the single nested-DTO assembly point described in the project's `CLAUDE.md`
- 11 of its 31 connections are within `Entities.cs` itself (field/type definitions)

## Notes

- No code was modified as part of this analysis.
- Raw graph output lives in `graphify-out/` at the repo root (not committed as part of this doc).
