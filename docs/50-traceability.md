# Requirements Traceability Matrix — GameHaven

All 18 requirements defined in [`docs/10-requirements.md`](10-requirements.md) are strictly mapped to automated test suites or deterministic build verifications.

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
| **REQ-013** | Canonical URL | Automated Unit Test | `GameHaven.UnitTests/Seo/SeoBaselineTests.cs::Canonical_Is_The_Host_Root` | PASSED |
| **REQ-014** | Description | Automated Unit Test | `GameHaven.UnitTests/Seo/SeoBaselineTests.cs::Description_Is_Single_And_Within_Snippet_Length` | PASSED |
| **REQ-015** | Social Cards | Automated Unit Test | `GameHaven.UnitTests/Seo/SeoBaselineTests.cs::Open_Graph_Matches_The_Canonical`, `Twitter_Card_Is_Complete` | PASSED |
| **REQ-016** | Structured Data | Automated Unit Test | `GameHaven.UnitTests/Seo/SeoBaselineTests.cs::JsonLd_Parses_And_Names_The_Author` | PASSED |
| **REQ-017** | Crawl Files | Automated Unit Test | `GameHaven.UnitTests/Seo/SeoBaselineTests.cs::Robots_Advertises_The_Sitemap_And_Blocks_Nothing`, `Sitemap_Lists_The_Canonical_Root`, `LlmsTxt_Starts_With_A_Heading` | PASSED |
| **REQ-018** | Author Link | Automated Unit Test | `GameHaven.UnitTests/Seo/SeoBaselineTests.cs::Author_Link_Points_To_The_Main_Site` | PASSED |

**Coverage Summary**: 18/18 requirements verified (100% coverage across 34 automated backend tests and frontend validation).
