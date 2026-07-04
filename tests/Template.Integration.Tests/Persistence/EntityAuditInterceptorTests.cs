namespace Template.Integration.Tests.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Template.Domain.Entities;
using Template.Infrastructure.Persistence;
using Template.Integration.Tests.Fixtures;
using Testcontainers.PostgreSql;

public class EntityAuditInterceptorTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres;
    private readonly CustomWebApplicationFactory _factory;

    public EntityAuditInterceptorTests()
    {
        _postgres = new PostgreSqlBuilder("postgres:16-alpine")
            .WithDatabase("template")
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
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        await _factory.DisposeAsync();
        await _postgres.DisposeAsync();
    }

    [Fact]
    public async Task SaveChangesAsync_Sets_Audit_Timestamps_On_Added_And_Modified_Entity()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var todo = new Todo("Audit test");
        await dbContext.Todos.AddAsync(todo);
        await dbContext.SaveChangesAsync();

        Assert.NotEqual(DateTime.MinValue, todo.CreatedAt);
        Assert.Equal(todo.CreatedAt, todo.UpdatedAt);

        DateTime addedUpdatedAt = todo.UpdatedAt!.Value;
        System.Threading.Thread.Sleep(10);

        todo.Complete();
        await dbContext.SaveChangesAsync();

        Assert.NotNull(todo.UpdatedAt);
        Assert.True(todo.UpdatedAt > addedUpdatedAt);
    }
}
