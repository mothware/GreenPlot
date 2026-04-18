# The Patch

**Garden Planning & Seed Starting Platform**

The Patch helps home and family gardeners manage the full lifecycle of their garden: from selecting seed varieties and planning bed layouts, through seed starting and germination tracking, to in-season care and season-over-season analytics.

## Architecture

| Layer | Technology |
|---|---|
| Backend API | ASP.NET Core 9 Web API |
| Database | PostgreSQL 16 |
| ORM | Entity Framework Core 9 |
| Frontend | React 19 + TypeScript + Vite |
| Styling | Tailwind CSS (warm paper palette, Fraunces/Instrument Sans fonts) |
| Drag & Drop | dnd-kit |
| State | TanStack Query + Zustand |
| Charts | Recharts |
| Auth | ASP.NET Identity + JWT |
| Background jobs | Hangfire |
| Storage | Azure Blob / Azurite (dev) |

## Project Structure

```
ThePatch (repo: GreenPlot)
├── src/
│   ├── ThePatch.Domain/         # Entities, enums, domain exceptions
│   ├── ThePatch.Application/    # MediatR handlers, DTOs, commands, queries, interfaces
│   ├── ThePatch.Infrastructure/ # EF Core DbContext, services (OpenFarm, USDA, storage)
│   └── ThePatch.Api/            # Controllers, auth, middleware, Hangfire, Program.cs
├── client/                      # React + TypeScript PWA
│   └── src/
│       ├── api/                 # Axios API clients (plants, varieties, gardens, plantings, calendar)
│       ├── features/            # catalog (+ seed packet modal), garden, bed-designer, calendar, auth
│       ├── stores/              # Zustand stores (auth, garden)
│       └── types/               # Shared TypeScript types
├── docker-compose.yml           # Dev services: PostgreSQL, Redis, Azurite
├── docker-compose.full.yml      # Full stack with API + client containers
└── ThePatch.sln
```

## Local Development Setup

### Prerequisites

- .NET 9 SDK
- Node.js 22+
- Docker (for database + storage)

### 1. Start dev services

```bash
docker compose up -d
```

This starts PostgreSQL (port 5432), Redis (6379), and Azurite blob storage (10000).

### 2. Configure secrets

```bash
cd src/ThePatch.Api
dotnet user-secrets set "Jwt:Key" "your-secret-key-at-least-32-characters-long"
```

### 3. Run the API

```bash
cd src/ThePatch.Api
dotnet run
```

API available at `http://localhost:5000`. Swagger UI at `http://localhost:5000/swagger`.

The database schema is created automatically on first run (`EnsureCreated`). Seed the plant catalog by triggering the **ImportOpenFarmDataJob** and **ImportUsdaPlantsJob** from the Hangfire dashboard at `/hangfire`. OpenFarm covers ~80 crops; USDA enrichment fills in scientific names, families, and hardiness zones.

### 4. Run the frontend

```bash
cd client
cp .env.example .env.local
npm install
npm run dev
```

Frontend available at `http://localhost:5173`.

## Decisions made

| Decision | Choice |
|---|---|
| Audience | Personal / family use — no billing, multi-user architecture retained |
| Name | **The Patch** |
| Seed catalog depth | Deep — full OpenFarm crop list + USDA PLANTS taxonomy enrichment |
| Companion rules | All rules shipped with Traditional / Research-supported / Community labels |
| Photo storage | Per-user quota (default 10 GB, configurable via `Storage:PhotoQuotaGb`); tiers deferred |
| Variety contributions | Users can add varieties from seed packets (inventory tracked as SeedLots) |
| Auth | Email + password, JWT 4-hour expiry — same pattern as NarthexLife |
| iOS wrapper | Deferred to Phase 5 |

## Still open

- Domain name / trademark for "The Patch"
- iOS-specific features (Capacitor in Phase 5?)

## Development Roadmap

| Phase | Goal | Status |
|---|---|---|
| 1 — Foundation | Auth, garden/bed creation, plant catalog, activity log | 🚧 In Progress |
| 2 — Intelligence | Auto seed-start dates, germination tracking, reminders | Planned |
| 3 — Visual Designer | Drag-and-drop bed layout, square-foot mode | Scaffolded |
| 4 — Companion & Rotation | Companion warnings, crop rotation engine | Planned |
| 5 — Analytics & Polish | Harvest analytics, season comparison, community features | Planned |
