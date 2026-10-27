# Operational Runbook — GameHaven

## 1. Port Reservations

Registered in `D:\Development\Tools\dev-ports.json`:

| Service | Port | Protocol | Path / Description |
| :--- | :--- | :--- | :--- |
| **GameHaven.Client** | `5340` | HTTP | Vue 3 + TypeScript Storefront |
| **GameHaven.Api** | `8180` | HTTP | ASP.NET Core Minimal API & Swagger at `/swagger` |

## 2. Local Execution

### Backend API (.NET 10)

```powershell
# Restore and run tests
dotnet test GameHaven.slnx

# Run the API on port 8180
dotnet run --project GameHaven.Api
```

Swagger UI will be accessible at: `http://localhost:8180/swagger`.

### Frontend Client (Vue 3)

```powershell
cd GameHaven.Client

# Install dependencies
npm install

# Start development server on port 5340
npm run dev

# Run production build
npm run build
```

The client will open at `http://localhost:5340`. If the API is offline, it automatically falls back to the embedded snapshot without crashing.

## 3. Automated Verification Gate

```powershell
# Complete backend test suite (domain + architecture fitness)
dotnet test GameHaven.slnx -c Release

# Client typecheck and production bundling
cd GameHaven.Client
npm run build
```
