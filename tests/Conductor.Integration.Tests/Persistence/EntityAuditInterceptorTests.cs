namespace Conductor.Integration.Tests.Persistence;

using Conductor.Domain.Entities;
using Conductor.Infrastructure.Persistence;
using Conductor.Integration.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

public class EntityAuditInterceptorTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly CustomWebApplicationFactory _factory;

    public EntityAuditInterceptorTests()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("conductor")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

        _factory = new CustomWebApplicationFactory();
    }

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _factory.ConnectionString = _postgres.GetConnectionString();

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConductorDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public Task DisposeAsync()
    {
        _factory.Dispose();
        return _postgres.DisposeAsync().AsTask();
    }

    [Fact]
    public async Task SaveChangesAsync_Sets_UpdatedAt_On_Modified_Entity()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ConductorDbContext>();

        var todo = new Todo("Audit test");
        await dbContext.Todos.AddAsync(todo);
        await dbContext.SaveChangesAsync();

        Assert.Null(todo.UpdatedAt);

        todo.Complete();
        await dbContext.SaveChangesAsync();

        Assert.NotNull(todo.UpdatedAt);
    }
}
