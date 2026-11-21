# ADR-0001: Clean Architecture Migration from WPF Monolith

- **Status**: Accepted
- **Deciders**: naindev
- **Date**: 2026-10-27

## Context

The legacy GameHaven desktop solution was written in .NET Framework 4.7.2 with WPF. Domain logic, data access, and UI rendering were deeply entangled in code-behind XAML files, preventing cross-platform usage and making automated unit testing virtually impossible.

## Decision

Deconstruct the monolith following Clean Architecture principles:
1. Extract a pure `GameHaven.Domain` library with zero dependencies on frameworks or UI.
2. Introduce an `Application` abstraction layer defining repository contracts.
3. Isolate data persistence inside an `Infrastructure` project utilizing EF Core and SQLite.
4. Expose business capabilities via a lightweight ASP.NET Core minimal API in `GameHaven.Api`.
5. Preserve the legacy desktop interface in `GameHaven.DesktopUI` as a reference consumer for migration parity checks.

## Consequences

- **Positive**:
  - Full cross-platform support across Linux, macOS, and Windows.
  - Automated testing of domain entities and arithmetic invariants in isolation.
  - Boundary isolation enforced via continuous `NetArchTest` fitness functions.
- **Negative / Trade-offs**:
  - Increased number of projects in the solution (`slnx`).
