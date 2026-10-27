# ADR-0002: Client Resilience with Local Catalog Snapshot

- **Status**: Accepted
- **Deciders**: naindev
- **Date**: 2026-10-27

## Context

Demonstrating client applications to prospective users or reviewers can fail if the backend API service is offline, cold-starting, or undergoing local maintenance. For a zero-cost portfolio presentation on GitHub Pages, hosting continuous live backends may not always be warranted.

## Decision

Implement an offline resilient fallback strategy in `GameHaven.Client`:
1. On startup, the web client attempts a probe request to `http://localhost:8180/api/games` (or the configured API endpoint).
2. If the API is available, the client operates in live mode with database synchronization.
3. If the request fails or times out, the client transitions to an offline snapshot mode using embedded initial data (`src/data/mockGames.ts`), displaying an informative status badge in the header without disrupting user navigation or cart management.

## Consequences

- **Positive**:
  - 100% resilient public demonstrations with zero paid hosting infrastructure.
  - Transparent status feedback to the user via status indicator pills.
- **Negative / Trade-offs**:
  - Mutations in offline snapshot mode are volatile and stored in local state only.
