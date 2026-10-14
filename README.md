# GameHaven · Indie Game Hub & Marketplace

An enterprise-grade indie game marketplace demonstrating the complete architectural modernization of a legacy desktop application (.NET 4.7.2 / WPF) into a cross-platform distributed solution built with **.NET 10 LTS**, **Clean Architecture**, **Domain-Driven Design (DDD)**, and **Vue 3 + TypeScript**.

## Architecture & Solution Structure

The solution (`GameHaven.slnx`) is organized under strict Clean Architecture boundaries:

| Layer | Technology | Purpose |
| :--- | :--- | :--- |
| **`GameHaven.Domain`** | C# / .NET 10 | Pure business entities (`Game`, `Cart`, `UserLibrary`) with strict domain invariants |
| **`GameHaven.Application`** | C# / .NET 10 | Use cases and repository contracts (`IGameRepository`, `ICartRepository`, `ILibraryRepository`) |
| **`GameHaven.Infrastructure`** | C# / .NET 10 / EF Core | Data persistence (SQLite / SQL Server), migrations, and test data seeding |
| **`GameHaven.Api`** | ASP.NET Core (.NET 10) | RESTful API with fixed-window rate limiting, OpenAPI/Swagger, and health checks |
| **`GameHaven.UnitTests`** | xUnit / FluentAssertions | Comprehensive unit test suite covering domain invariants and monetary calculations |
| **`GameHaven.Client`** | Vue 3 / TypeScript / Vite | Modern, responsive web storefront with offline fallback and cart checkout workflow |
| **`GameHaven.DesktopUI`** | .NET 10 Windows (WPF) | Preserved legacy desktop client for migration parity testing |

## Case Study

For the detailed enterprise migration methodology, architectural trade-offs, and before/after metrics:
- Read [`docs/00-modernization-case-study.md`](docs/00-modernization-case-study.md).

## Local Development & Ports

Synchronized with `Tools/dev-ports.json`:
- **Web Client (Vue 3)**: Port `5340` (`http://localhost:5340`)
- **Backend API (.NET 10)**: Port `8180` (`http://localhost:8180`)

### Running the API

```powershell
dotnet run --project GameHaven.Api
```

Swagger UI will be available at `http://localhost:8180/swagger`.

### Running the Web Client

```powershell
cd GameHaven.Client
npm install
npm run dev
```

### Running Tests

```powershell
dotnet test GameHaven.slnx
```
