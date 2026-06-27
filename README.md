# Conductor

Open source alternative to VisualCron. Distributed job scheduler based on agents.

> **Current scope:** backend + infrastructure. The agent and frontend are not part of this template yet.

## Structure

```
Conductor/
├── src/
│   ├── Template.Domain/          # Entities, domain events, repository interfaces
│   ├── Template.Application/     # Commands, queries, handlers (Wolverine)
│   ├── Template.Infrastructure/  # EF Core, persistence, repository implementations
│   ├── Template.Api/             # ASP.NET Core Minimal API host
│   └── shared/
│       └── Template.Contracts/   # Shared DTOs
├── tests/
│   ├── Template.Domain.Tests/
│   ├── Template.Application.Tests/
│   └── Template.Integration.Tests/
├── docker/
│   └── Dockerfile
├── docker-compose.yml
├── Directory.Build.props
├── Directory.Packages.props
└── global.json
```

## Stack

- [.NET 10](https://dotnet.microsoft.com/download)
- [PostgreSQL 16](https://www.postgresql.org/)
- [Entity Framework Core 9](https://learn.microsoft.com/ef/core/)
- [WolverineFx](https://wolverinefx.net/) (mediator + domain event routing)
- [Mapster](https://github.com/MapsterMapper/Mapster)
- [Scalar](https://scalar.com/) OpenAPI reference

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker + Docker Compose](https://docs.docker.com/get-docker/) (for the provided PostgreSQL container)

## Getting Started

### Run with Docker Compose

```bash
docker-compose up -d
```

This starts PostgreSQL and the API. The API applies pending migrations automatically on startup.

- API: http://localhost:5000
- Health check: http://localhost:5000/health
- OpenAPI (development only): http://localhost:5000/scalar/v1

### Run locally

1. Start PostgreSQL:

   ```bash
   docker-compose up -d postgres
   ```

2. Run the API:

   ```bash
   dotnet run --project src/Template.Api
   ```

The API expects a connection string in `DatabaseConfig:ConnectionString`. By default `appsettings.Development.json` points to `localhost:5432`.

## Database Migrations

Create a new migration:

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Template.Infrastructure/Template.Infrastructure.csproj \
  --startup-project src/Template.Api/Template.Api.csproj \
  --output-dir Persistence/Migrations
```

Apply migrations locally:

```bash
dotnet ef database update \
  --project src/Template.Infrastructure/Template.Infrastructure.csproj \
  --startup-project src/Template.Api/Template.Api.csproj
```

Migrations are applied automatically when the API starts inside Docker.

## Build & Test

```bash
dotnet build
dotnet test
```

Integration tests use Testcontainers to spin up a throwaway PostgreSQL container, so Docker must be running.

## API Endpoints

| Method | Endpoint      | Description                   |
|--------|---------------|-------------------------------|
| GET    | `/health`     | Database health check         |
| GET    | `/todos`      | Paginated list of todos       |
| POST   | `/todos`      | Create a new todo             |

Example:

```bash
curl -X POST http://localhost:5000/todos \
  -H "Content-Type: application/json" \
  -d '{"title":"Buy milk"}'
```

## Architecture Notes

- `Template.Domain` has no dependency on EF Core or ASP.NET Core.
- `Template.Application` uses Wolverine as a mediator and returns domain events from handlers.
- `Template.Infrastructure` implements repositories and EF Core configuration.
- Wolverine is configured to use service location only for `ConductorDbContext`, because `AddDbContext` registers `DbContextOptions<T>` as an opaque lambda factory.

## License

MIT
