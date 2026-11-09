# Requirements Specification — GameHaven

All requirements follow the Easy Approach to Requirements Syntax (EARS).

## 1. Domain & Invariant Requirements

- **REQ-001 (Monetary Calculation Invariant)**: When a `Game` entity is instantiated or updated, the system shall require that `BasePrice` is greater than or equal to zero and that `DiscountPercentage` is bounded between 0 and 100, calculating `CurrentPrice` deterministically as `BasePrice * (1 - DiscountPercentage / 100)`.
- **REQ-002 (Cart Exclusivity Invariant)**: When a client attempts to add a game to an active `Cart`, the system shall prevent duplicate entries of the same title, rejecting attempts with an `InvalidOperationException`.
- **REQ-003 (Cart Arithmetic Balance)**: When items are added or removed from a `Cart`, the system shall calculate `TotalPrice` as the exact arithmetic sum of each item's price at the time of addition.
- **REQ-004 (Digital Library Exclusivity)**: When an order is completed, the system shall add the games to the user's `UserLibrary` without duplication; any game already owned shall not be added again.

## 2. Architectural Fitness Requirements

- **REQ-005 (Domain Isolation)**: When automated architecture tests execute, the test suite shall assert that `GameHaven.Domain` has zero package or project dependencies on `GameHaven.Application`, `GameHaven.Infrastructure`, `GameHaven.Api`, `GameHaven.DesktopUI`, `GameHaven.Client`, and `Microsoft.EntityFrameworkCore`.
- **REQ-006 (Application Layer Decoupling)**: When automated architecture tests execute, the test suite shall assert that `GameHaven.Application` has zero dependencies on `GameHaven.Infrastructure`, `GameHaven.Api`, or UI projects.
- **REQ-007 (Infrastructure Boundary Guard)**: When automated architecture tests execute, the test suite shall assert that `GameHaven.Infrastructure` has zero dependencies on `GameHaven.Api` or UI projects.

## 3. API & Gateway Requirements

- **REQ-008 (Fixed Window Rate Limiting)**: While processing incoming HTTP requests, the `GameHaven.Api` gateway shall enforce a fixed window rate limit of 60 requests per minute with a queue limit of 2, returning HTTP status `429 Too Many Requests` when limits are exceeded.
- **REQ-009 (RESTful Catalog & Checkout Endpoints)**: When clients request `/api/games`, `/api/cart`, or `/api/checkout`, the API shall provide compliant JSON responses matching OpenAPI specifications.

## 4. Frontend & Ecosystem Requirements

- **REQ-010 (Client Offline Resilience)**: When the backend API is unreachable or in demonstration mode, `GameHaven.Client` shall fall back to an embedded client-side catalog snapshot, preserving store browsing, cart mutations, and mock checkout.
- **REQ-011 (Ecosystem Bar Integration)**: While running in any browser context, `GameHaven.Client` shall mount `<nain-ecosystem-bar>` to provide unified cross-portfolio navigation and telemetry status.
- **REQ-012 (Development Port Governance)**: When running in local development mode, the system shall bind strictly to port `8180` for the .NET API and port `5340` for the Vue 3 web client, matching `dev-ports.json`.

## 5. Discoverability Requirements

The public demo is the static client published to GitHub Pages, so these requirements apply to the files the browser receives (`GameHaven.Client/index.html` and `GameHaven.Client/public/`).

- **REQ-013 (Canonical URL)**: When a crawler fetches the client entry page, the page shall declare exactly one canonical link equal to `https://gamehaven.naindev.com/`.
- **REQ-014 (Description)**: When a crawler fetches the client entry page, the page shall carry exactly one meta description of 70 to 160 characters.
- **REQ-015 (Social Cards)**: When a link to the demo is shared, the page shall expose Open Graph tags (`og:url` equal to the canonical, absolute `og:image`) and a `summary_large_image` Twitter card.
- **REQ-016 (Structured Data)**: When a crawler fetches the client entry page, the page shall carry one `SoftwareApplication` JSON-LD block whose author is `https://www.naindev.com/#person`, with `<` escaped.
- **REQ-017 (Crawl Files)**: When a crawler requests the site root files, the site shall serve `robots.txt` (allow all, advertising the sitemap), `sitemap.xml` (the canonical root) and `llms.txt` (Markdown, H1 first) as real files, not the SPA fallback page.
- **REQ-018 (Author Link)**: When a crawler fetches the client entry page, the page shall link `rel="author"` to `https://www.naindev.com/`.
