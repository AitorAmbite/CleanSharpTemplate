# Template Review — Conductor Backend Template

> Review date: 2026-06-27  
> Branch: `Template`  
> Scope: backend + infrastructure only (frontend ignored per request)

## Overall Score: 48 / 100

This is a well-intentioned architectural skeleton, but it is still too empty and operationally immature to be a solid medium-project template. The layer separation is good, yet several critical pieces are missing or incomplete.

---

## What is good

- Clean layer separation: `Domain`, `Application`, `Infrastructure`, `Api`, `Contracts`, `Agent`.
- Modern solution file (`.slnx`).
- Multi-database wiring ready (SQLite / PostgreSQL).
- Modern stack choices: Wolverine, Quartz, Mapster, Scalar, Minimal APIs.
- `IUnitOfWork`, `IRepository<T>`, `AggregateRoot`, `DomainEvent` base classes exist.
- Agent is separated as its own Worker Service.

---

## Critical fixes (do first)

### 1. Fix NuGet warnings and vulnerabilities
- `SQLitePCLRaw.lib.e_sqlite3` 2.1.10 has a known high-severity vulnerability (GHSA-2m69-gcr7-jv3q).
- `Microsoft.CodeAnalysis.Workspaces.MSBuild` 4.8.0 conflicts with resolved 5.0.0 packages.
- Align or remove unused packages.

### 2. Remove `Microsoft.EntityFrameworkCore` from `Template.Application`
- `Application` currently depends on EF Core because `PaginatedList` and `QueryableExtensions` expose `IQueryable<T>`.
- This breaks strict Clean Architecture. See "Recommended approach for pagination" below.

### 3. Complete the repository pattern
- `IRepository<T>` is defined but never registered in DI.
- `Repository<T>` is abstract and has no concrete implementations.
- Register a generic repository or define aggregate-specific repositories.

### 4. Add a minimal end-to-end feature example
- Currently there are no entities, handlers, endpoints, or migrations.
- Add one small feature (e.g., a `Todo` or `Job` aggregate) that demonstrates the full wiring: entity → repository → handler → endpoint → migration → test.

---

## Important fixes

### 5. Centralize .NET build configuration
- Add `global.json` to pin the SDK version.
- Add `Directory.Build.props` for shared properties (`Nullable`, `ImplicitUsings`, `TreatWarningsAsErrors`, test `IsPackable`).
- Add `Directory.Packages.props` to centralize NuGet versions and avoid mismatches.

### 6. Add `.editorconfig`
- Consistent code style across contributors.

### 7. Wire up domain event dispatching
- `AggregateRoot` can collect events, but nothing publishes or clears them.
- Add a dispatcher via EF interceptor or domain service.

### 8. Start Quartz as a hosted service
- Missing `AddQuartzServer()` in `Application` DI.
- Without it, the scheduler does not run.

### 9. Complete or remove `Template.Contracts`
- The project is empty. If the agent will share contracts, add example DTOs/messages.

### 10. Clean up orphan test folders
- `tests/server/Template.Functional.Tests/` and `tests/server/Template.Infrastructure.Tests/` exist physically but are not in the solution and are empty.
- Either add `.csproj` files and include them in `.slnx`, or delete them.

---

## Infrastructure & DevOps

### 11. Add Docker support
- Add `docker-compose.yml` with PostgreSQL at minimum.
- Add multi-stage `Dockerfile`(s) for the API and Agent.

### 12. Add CI/CD
- GitHub Actions workflow: build, `dotnet test`, NuGet vulnerability scan (`dotnet list package --vulnerable`).

### 13. Fix secret management
- Move the PostgreSQL password out of `appsettings.json`.
- Use Secret Manager or environment variables.
- Add `UserSecretsId` to `Template.Api`.

### 14. Add health checks
- `AddHealthChecks()` and a `/health` endpoint in the API.

---

## Template quality

### 15. Add real tests instead of placeholders
- Replace the four empty `UnitTest1.cs` files with at least:
  - One domain unit test.
  - One application/handler test.
  - One integration test with SQLite in-memory or Testcontainers.
  - One functional test using `WebApplicationFactory`.

### 16. Add global exception handling
- Middleware or extension to catch exceptions and return consistent Problem Details responses.

### 17. Add structured logging
- Consider Serilog as the default logger in the template.

### 18. Update `README.md`
- Remove references to `docker/` and migrations that do not exist yet.
- Document the actual getting-started steps for the template.

---

## Recommended approach for pagination (without `IQueryable` in Application)

Move pagination into the repository implementation while keeping `Application` pure.

### In `Domain` / `Application`

```csharp
public interface ISpecification<T>
{
    Expression<Func<T, bool>>? Criteria { get; }
    List<Expression<Func<T, object>>> Includes { get; }
    List<(Expression<Func<T, object>> KeySelector, bool Descending)> OrderBy { get; }
}

public interface IRepository<T> where T : Entity
{
    Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<T>> ListAsync(ISpecification<T>? spec = null, CancellationToken ct = default);
    Task<PaginatedList<T>> GetPagedAsync(ISpecification<T>? spec, int page, int pageSize, CancellationToken ct = default);
    Task<int> CountAsync(ISpecification<T>? spec = null, CancellationToken ct = default);
}
```

### In `Infrastructure`

`Repository<T>` builds `IQueryable` internally, applies the specification, projects with `ProjectToType<TDto>()` if needed, and returns a materialized `PaginatedList<T>`.

### Tradeoff

- You lose ad-hoc `IQueryable` composition in handlers.
- You keep SQL-side pagination and filtering, and `Application` stays independent of EF Core.

For very specific queries, add specialized repository methods rather than generic specs.

---

## Suggested implementation order

1. Fix NuGet warnings/vulnerabilities and add `global.json` / `Directory.Build.props` / `Directory.Packages.props`.
2. Remove EF Core from `Application` and refactor pagination to specification pattern.
3. Register `IRepository<T>` in DI and add a concrete generic repository.
4. Add one end-to-end feature example with entity, migration, handler, endpoint, and contract.
5. Add real tests for that feature across Domain/Application/Integration/Functional layers.
6. Wire up domain event dispatching and Quartz hosted service.
7. Add Docker, health checks, exception handling, and structured logging.
8. Add GitHub Actions CI workflow.
9. Update README and remove orphan folders.
