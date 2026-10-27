# GameHaven · Hub y Marketplace de Juegos Indie

[![English](https://img.shields.io/badge/Language-English-blue?style=flat-square)](README.md)
[![Español](https://img.shields.io/badge/Idioma-Espa%C3%B1ol-emerald?style=flat-square)](README.es.md)
[![Architecture Tests](https://img.shields.io/badge/Architecture%20Tests-NetArchTest%20enforced-80FF00?style=flat-square&logo=checkmarx&logoColor=black)](GameHaven.UnitTests/Architecture/CleanArchitectureTests.cs)
[![Live Demo](https://img.shields.io/badge/Demo%20Online-gamehaven.naindev.com-38bdf8?style=flat-square&logo=googlechrome&logoColor=white)](https://gamehaven.naindev.com/)
[![Vue 3](https://img.shields.io/badge/Vue%203-TypeScript-emerald?style=flat-square)](https://gamehaven.naindev.com/)
[![Zero Cost](https://img.shields.io/badge/Hosting-0%2C00%20%E2%82%AC%20FOSS-emerald?style=flat-square)](https://gamehaven.naindev.com/)

**Demo Online Interactiva:** [https://gamehaven.naindev.com/](https://gamehaven.naindev.com/) (Catálogo interactivo, carrito y biblioteca en cliente, coste 0,00 €)

Marketplace de videojuegos indie de nivel empresarial que demuestra la modernización arquitectónica completa de una aplicación de escritorio heredada (.NET 4.7.2 / WPF) hacia una solución distribuida multiplataforma construida con **.NET 10 LTS**, **Clean Architecture**, **Diseño Guiado por el Dominio (DDD)** y **Vue 3 + TypeScript**.

## Arquitectura y Estructura de la Solución

La solución (`GameHaven.slnx`) está organizada bajo estrictas fronteras de Clean Architecture verificadas por pruebas de aptitud automáticas (`NetArchTest`):

| Capa | Tecnología | Propósito |
| :--- | :--- | :--- |
| **`GameHaven.Domain`** | C# / .NET 10 | Entidades puras de negocio (`Game`, `Cart`, `UserLibrary`) con invariantes estrictos de dominio |
| **`GameHaven.Application`** | C# / .NET 10 | Casos de uso y contratos de repositorio (`IGameRepository`, `ICartRepository`, `ILibraryRepository`) |
| **`GameHaven.Infrastructure`** | C# / .NET 10 / EF Core | Persistencia de datos (SQLite / SQL Server), migraciones y siembra de datos de prueba |
| **`GameHaven.Api`** | ASP.NET Core (.NET 10) | API RESTful con limitador de tasa de ventana fija, OpenAPI/Swagger y comprobaciones de salud |
| **`GameHaven.UnitTests`** | xUnit / FluentAssertions / NetArchTest | Suite integral de pruebas unitarias y de arquitectura que blinda los límites entre capas |
| **`GameHaven.Client`** | Vue 3 / TypeScript / Vite | Tienda web moderna y adaptable con respaldo sin conexión y flujo de compra en carrito |
| **`GameHaven.DesktopUI`** | .NET 10 Windows (WPF) | Cliente de escritorio histórico preservado para pruebas de paridad en la migración |

## Caso de Estudio

Para conocer la metodología detallada de migración empresarial, los balances arquitectónicos y las métricas antes/después:
- Consulta [`docs/00-modernization-case-study.md`](docs/00-modernization-case-study.md).

## Desarrollo Local y Puertos

Sincronizado con `Tools/dev-ports.json`:
- **Cliente Web (Vue 3)**: Puerto `5340` (`http://localhost:5340`)
- **Backend API (.NET 10)**: Puerto `8180` (`http://localhost:8180`)

### Ejecución de la API

```powershell
dotnet run --project GameHaven.Api
```

La interfaz Swagger UI estará disponible en `http://localhost:8180/swagger`.

### Ejecución del Cliente Web

```powershell
cd GameHaven.Client
npm install
npm run dev
```

### Ejecución de Pruebas

```powershell
dotnet test GameHaven.slnx
```
