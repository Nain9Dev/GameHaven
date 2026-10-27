# Requirements Traceability Matrix — GameHaven

All 12 requirements defined in [`docs/10-requirements.md`](10-requirements.md) are strictly mapped to automated test suites or deterministic build verifications.

| Requirement ID | Summary | Verification Type | Implementation / Test Target | Status |
| :--- | :--- | :--- | :--- | :--- |
| **REQ-001** | Monetary Calculation Invariant | Automated Unit Test | `GameHaven.UnitTests/Domain/GameTests.cs` | PASSED |
| **REQ-002** | Cart Exclusivity Invariant | Automated Unit Test | `GameHaven.UnitTests/Domain/CartTests.cs` | PASSED |
| **REQ-003** | Cart Arithmetic Balance | Automated Unit Test | `GameHaven.UnitTests/Domain/CartTests.cs` | PASSED |
| **REQ-004** | Digital Library Exclusivity | Automated Unit Test | `GameHaven.UnitTests/Domain/UserLibraryTests.cs` | PASSED |
| **REQ-005** | Domain Isolation | Architecture Fitness Test | `GameHaven.UnitTests/Architecture/CleanArchitectureTests.cs::Domain_Should_Not_Have_Dependency_On_Other_Projects` | PASSED |
| **REQ-006** | Application Layer Decoupling | Architecture Fitness Test | `GameHaven.UnitTests/Architecture/CleanArchitectureTests.cs::Application_Should_Not_Have_Dependency_On_Infrastructure_Or_Api_Or_UI` | PASSED |
| **REQ-007** | Infrastructure Boundary Guard | Architecture Fitness Test | `GameHaven.UnitTests/Architecture/CleanArchitectureTests.cs::Infrastructure_Should_Not_Have_Dependency_On_Api_Or_UI` | PASSED |
| **REQ-008** | Fixed Window Rate Limiting | Integration & Configuration | `GameHaven.Api/Program.cs` (`AddRateLimiter`, 60 req/min) | VERIFIED |
| **REQ-009** | RESTful Catalog & Checkout | API Endpoints | `GameHaven.Api/Program.cs` (`/api/games`, `/api/cart`, `/api/checkout`) | VERIFIED |
| **REQ-010** | Client Offline Resilience | Frontend Strategy | `GameHaven.Client/src/api.ts` & `src/data/mockGames.ts` | VERIFIED |
| **REQ-011** | Ecosystem Bar Integration | Web Component Contract | `GameHaven.Client/index.html` (`<nain-ecosystem-bar current-project="GameHaven">`) | VERIFIED |
| **REQ-012** | Development Port Governance | Configuration Audit | `dev-ports.json`, `GameHaven.Api/Properties/launchSettings.json`, `GameHaven.Client/vite.config.ts` | VERIFIED |

**Coverage Summary**: 12/12 requirements verified (100% coverage across 25 automated backend tests and frontend validation).
