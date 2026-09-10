# Frontend Graph Analysis (Graphify)

## Setup

Built a fresh Graphify graph from this repo: 283 nodes, 437 edges, 12 communities.
AST extraction over 20 code files + semantic extraction over 4 doc files
(CLAUDE.md, README.md, docker-compose.yml, index.html).

Health check flagged 10 dangling-endpoint edges and 35 collapsed parallel edges —
expected artifacts of the undirected build, not corruption.

## `graphify path "DashboardPage" "AuthorizationsController.GetAll"`

No directed path exists; undirected shortest path is **4 hops**:

```
DashboardPage() --contains--> DashboardPage.tsx --lists--> Authorization (table/record)
    --creates--> AuthorizationsController.cs --contains--> AuthorizationsController
```

This traces the *data entity* linkage (both sides touch the `Authorization` record),
not a direct call edge.

## `graphify query "what calls api.authorizations.getAll"`

Graph returned 4 nodes via BFS but **did not resolve a direct call edge** from
`DashboardPage.tsx` to `.GetAll()` — the AST extractor doesn't bridge the frontend
`fetch` call in `client.ts` to the backend controller method (no cross-language
call edge type in this corpus).

### Ground-truth check (Grep, not graph)

The only caller is confirmed at `frontend/src/pages/DashboardPage.tsx:17`:

```ts
const data = await api.authorizations.getAll(statusFilter || undefined)
```

which calls into `frontend/src/api/client.ts` → `GET /api/authorizations` →
`AuthorizationsController.GetAll()`
(`backend/PriorAuth.API/Controllers/AuthorizationsController.cs:22`).

## Caveat

The graph is code-structure-only and doesn't model HTTP-route-to-controller-action
linkage, so for this specific frontend↔backend call chain the grep result is more
reliable than the graph traversal.
