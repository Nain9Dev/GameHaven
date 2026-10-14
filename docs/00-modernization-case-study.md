# GameHaven: Modernización Enterprise de .NET 4.7.2 a .NET 10 LTS

## 1. Resumen Ejecutivo

Este caso de estudio documenta la refactorización arquitectónica completa de **GameHaven**, transformando una aplicación de escritorio monolítica heredada (*legacy*) construida sobre **.NET Framework 4.7.2 y WPF** en una plataforma distribuida moderna, desacoplada y multiplataforma basada en **.NET 10 LTS**, **Clean Architecture**, **Domain-Driven Design (DDD)** y un cliente web reactivo en **Vue 3 + TypeScript**.

El proyecto sirve como demostrador de referencia técnica para el servicio de **Modernización de .NET Heredado y Arquitectura Enterprise** de NainDev.

---

## 2. Comparativa Arquitectónica: Antes vs Después

| Dimensión | Estado Heredado (Legacy) | Estado Modernizado (.NET 10) |
| :--- | :--- | :--- |
| **Runtime & SDK** | .NET Framework 4.7.2 (propietario Windows) | **.NET 10 LTS** (Multiplataforma Linux / Windows / macOS) |
| **Arquitectura** | Monolito de escritorio acoplado (WPF) | **Clean Architecture** (Dominio, Aplicación, Infraestructura, API, Web) |
| **Capa de Dominio** | Lógica dispersa en code-behind XAML | **Entidades ricas con invariantes puros** (`Game`, `Cart`, `UserLibrary`) |
| **Acceso a Datos** | Consultas directas ADO.NET / SQL Server acopladas | **EF Core + SQLite/SQL Server** con patrón Repositorio y Unit of Work |
| **Interfaz de Usuario** | Desktop WPF (Windows Presentation Foundation) | **SPA moderna en Vue 3 + TypeScript + Vite** (móvil y escritorio) |
| **Testing** | 0 pruebas unitarias automatizadas | **Suite xUnit + FluentAssertions** (cobertura de invariantes de dominio) |
| **Despliegue & Portabilidad** | Instalador MSI / binarios Windows | **Contenedores Docker** y despliegue estático Web |

---

## 3. Arquitectura del Sistema (Clean Architecture)

```
GameHaven Solution (GameHaven.slnx)
│
├── GameHaven.Domain/          # Núcleo de negocio puro sin dependencias externas
│   ├── Entities/              # Cart, Game, UserLibrary, CartItem, OwnedGame
│   └── Enums/                 # Categorías y estados
│
├── GameHaven.Application/     # Casos de uso e interfaces de servicio
│   └── Interfaces/            # IGameRepository, ICartRepository, ILibraryRepository
│
├── GameHaven.Infrastructure/  # Persistencia y adaptadores
│   ├── Persistence/           # GameHavenDbContext, DataSeeder, Repositorios EF Core
│   └── DependencyInjection.cs # Registro modular de servicios
│
├── GameHaven.Api/             # API REST en .NET 10
│   ├── Program.cs             # Minimal APIs, RateLimiting, Swagger/OpenAPI, CORS
│   └── Properties/            # launchSettings.json (Puerto 8180)
│
├── GameHaven.UnitTests/       # Batería de pruebas automatizadas
│   ├── Domain/                # GameTests, CartTests, UserLibraryTests (xUnit)
│   └── 21/21 passing tests
│
├── GameHaven.Client/          # Frontend Web moderno
│   ├── src/                   # Vue 3, TypeScript, Vite (Puerto 5340)
│   └── dist/                  # Bundle de producción optimizado (92 KB)
│
└── GameHaven.DesktopUI/       # Interfaz heredada preservada como referencia histórica
```

---

## 4. Invariantes de Dominio y Reglas Deterministas

1. **Invariante de Moneda y Descuentos (`Game.cs`)**:
   - El precio base no puede ser negativo (`ArgumentException`).
   - El porcentaje de descuento está estrictamente acotado entre $0\%$ y $100\%$.
   - El precio actual se calcula analíticamente: $\text{CurrentPrice} = \text{BasePrice} \times (1 - \frac{\text{Discount}}{100})$.
2. **Invariante de Cesta Única (`Cart.cs`)**:
   - Una compra digital no admite copias duplicadas del mismo título en la misma transacción (`InvalidOperationException`).
   - El importe total es la suma determinista de los precios en el momento de agregado.
3. **Invariante de Biblioteca Digital (`UserLibrary.cs`)**:
   - Los juegos ya adquiridos no pueden recomprarse ni duplicarse en la biblioteca del usuario.

---

## 5. Verificación y Resultados Medidos

- **Compilación de la solución**: `dotnet build GameHaven.slnx` -> 0 advertencias, 0 errores.
- **Batería de pruebas unitarias**: 21 pruebas ejecutadas y superadas en 159 ms bajo `net10.0`.
- **Compilación frontend**: `npm run build` -> bundle generado en 655 ms (92 KB JS gzip 34 KB).
- **Protección de puertos**: API asignada a puerto 8180; Web client asignado a puerto 5340 (alineado con `dev-ports.json`).
