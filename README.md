# GreenPlot

**Garden Planning & Seed Starting Platform**

GreenPlot helps home and community gardeners manage the full lifecycle of their garden: from selecting seed varieties and planning bed layouts, through seed starting and germination tracking, to in-season care and season-over-season analytics.

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
GreenPlot/
├── src/
│   ├── GreenPlot.Domain/        # Entities, enums, domain exceptions
│   ├── GreenPlot.Application/   # MediatR handlers, DTOs, commands, queries, interfaces
│   ├── GreenPlot.Infrastructure/# EF Core DbContext, services (OpenFarm, USDA, storage)
│   └── GreenPlot.Api/           # Controllers, auth, middleware, Hangfire, Program.cs
├── client/                      # React + TypeScript PWA
│   └── src/
│       ├── api/                 # Axios API clients (plants, gardens, plantings, calendar)
│       ├── features/            # catalog, garden, bed-designer, calendar, auth
│       ├── stores/              # Zustand stores (auth, garden)
│       └── types/               # Shared TypeScript types
├── docker-compose.yml           # Dev services: PostgreSQL, Redis, Azurite
├── docker-compose.full.yml      # Full stack with API + client containers
└── GreenPlot.sln
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
cd src/GreenPlot.Api
dotnet user-secrets set "Jwt:Key" "your-secret-key-at-least-32-characters-long"
```

### 3. Run the API

```bash
cd src/GreenPlot.Api
dotnet run
```

API available at `http://localhost:5000`. Swagger UI at `http://localhost:5000/swagger`.

The database schema is created automatically on first run (`EnsureCreated`). Run the OpenFarm import job from the Hangfire dashboard to seed the plant catalog.

### 4. Run the frontend

```bash
cd client
cp .env.example .env.local
npm install
npm run dev
```

Frontend available at `http://localhost:5173`.

## Open Questions (from spec)

The following decisions are needed before Phase 1 ships — see §10 of the requirements document:

1. **Audience** — Personal, SaaS, or open source? Affects auth complexity and billing.
2. **Monetization** — Freemium tier boundaries if SaaS.
3. **Seed catalog depth** — 50 crops (MVP) or 200+ at launch?
4. **Companion planting rigor** — Ship all traditional rules with confidence labels, or research-supported only?
5. **Photo storage quotas** — Per-user limit model.
6. **Community contributions** — Allow user-submitted variety data?
7. **iOS native wrapper** — Capacitor in Phase 5?

## Development Roadmap

| Phase | Goal | Status |
|---|---|---|
| 1 — Foundation | Auth, garden/bed creation, plant catalog, activity log | 🚧 In Progress |
| 2 — Intelligence | Auto seed-start dates, germination tracking, reminders | Planned |
| 3 — Visual Designer | Drag-and-drop bed layout, square-foot mode | Scaffolded |
| 4 — Companion & Rotation | Companion warnings, crop rotation engine | Planned |
| 5 — Analytics & Polish | Harvest analytics, season comparison, community features | Planned |
