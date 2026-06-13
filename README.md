# Conductor

Open source alternative to VisualCron. Distributed job scheduler based on agents.

## Structure

```
Conductor/
├── src/
│   ├── server/
│   │   ├── Conductor.Domain/
│   │   ├── Conductor.Application/
│   │   ├── Conductor.Infrastructure/
│   │   └── Conductor.Api/
│   ├── agent/
│   │   └── Conductor.Agent/
│   └── shared/
│       └── Conductor.Contracts/
├── tests/
│   ├── server/
│   └── agent/
├── docker/
└── installer/
```

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker + Docker Compose](https://docs.docker.com/get-docker/)
- [PostgreSQL 15+](https://www.postgresql.org/) (or use the provided Docker Compose)

## Getting Started

```bash
# 1. Start PostgreSQL
docker-compose -f docker/docker-compose.yml up -d postgres

# 2. Run the API
dotnet run --project src/server/Conductor.Api

# 3. Run the agent (separate terminal)
dotnet run --project src/agent/Conductor.Agent
```

## Database Migrations

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> \
  --project src/server/Conductor.Infrastructure/Conductor.Infrastructure.csproj \
  --startup-project src/server/Conductor.Api/Conductor.Api.csproj \
  --output-dir Persistence/Migrations

# Apply migrations
dotnet ef database update \
  --project src/server/Conductor.Infrastructure/Conductor.Infrastructure.csproj \
  --startup-project src/server/Conductor.Api/Conductor.Api.csproj
```

## Build & Test

```bash
dotnet build
dotnet test
```

## License

MIT
