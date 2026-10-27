# Project Charter — GameHaven

## 1. Problem Statement & Mission

Legacy enterprise desktop applications built on .NET Framework 4.7.2 and WPF are tightly coupled to Windows host runtimes, lack automated testing, and resist cloud-ready deployments.

GameHaven demonstrates the end-to-end modernization of an indie game digital marketplace from a legacy WPF monolithic codebase to a cross-platform, high-performance distributed architecture powered by **.NET 10 LTS**, **Clean Architecture**, **Domain-Driven Design (DDD)**, and a decoupled **Vue 3 + TypeScript** web storefront.

## 2. Architectural Objectives

1. **Deterministic Business Core**: Encapsulate all monetary arithmetic and cart validation invariants inside a pure `GameHaven.Domain` project with zero dependencies on database engines, web frameworks, or UI code.
2. **Automated Architectural Fitness**: Prevent boundary regressions using continuous `NetArchTest` assertions in CI.
3. **Decoupled Modern Presentation**: Replace XAML code-behind with a reactive, accessible (WCAG 2.2 AA) Vue 3 SPA capable of running offline via cached catalog snapshots.
4. **Zero-Cost Local & Demo Hosting**: Operate under 0.00 EUR recurring infrastructure cost, utilizing embedded SQLite, in-memory test harnesses, and GitHub Pages static hosting.

## 3. Scope Boundaries

- **In Scope**:
  - Pure domain entities: `Game`, `Cart`, `UserLibrary`, `CartItem`, `OwnedGame`.
  - Repository interfaces in `GameHaven.Application`.
  - EF Core SQLite implementation and seed data in `GameHaven.Infrastructure`.
  - Minimal API REST gateway with rate limiting and OpenAPI scalar in `GameHaven.Api`.
  - Automated unit and architecture tests in `GameHaven.UnitTests`.
  - Responsive Vue 3 + TypeScript frontend in `GameHaven.Client`.
  - Legacy desktop client (`GameHaven.DesktopUI`) preserved for migration parity reference.
- **Out of Scope**:
  - Payment gateway merchant account integration (orders complete in-memory).
  - External distributed authentication services (runs standalone).
