namespace Template.Integration.Tests.Health;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Template.Api.Health;
using Template.Infrastructure.Persistence;

public class DatabaseHealthCheckTests
{
    [Fact]
    public async Task CheckHealthAsync_When_Database_Reachable_Returns_Healthy()
    {
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("healthy")
            .Options;
        await using var dbContext = new AppDbContext(options);
        var healthCheck = new DatabaseHealthCheck(dbContext);

        HealthCheckResult result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        Assert.Equal(HealthStatus.Healthy, result.Status);
        Assert.Equal("Database connection is healthy", result.Description);
    }

    [Fact]
    public async Task CheckHealthAsync_When_Database_Unreachable_Returns_Unhealthy()
    {
        // Use Npgsql with a non-listening port so CanConnectAsync fails fast.
        DbContextOptions<AppDbContext> options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=localhost;Port=1;Database=template;Username=postgres;Password=postgres;Timeout=1;CommandTimeout=1")
            .Options;
        await using var dbContext = new AppDbContext(options);
        var healthCheck = new DatabaseHealthCheck(dbContext);

        HealthCheckResult result = await healthCheck.CheckHealthAsync(new HealthCheckContext(), CancellationToken.None);

        Assert.Equal(HealthStatus.Unhealthy, result.Status);
        Assert.Equal("Unable to connect to the database", result.Description);
    }
}