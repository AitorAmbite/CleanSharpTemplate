namespace Template.Api.Health;

using Microsoft.Extensions.Diagnostics.HealthChecks;
using Template.Infrastructure.Persistence;

public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ConductorDbContext _dbContext;

    public DatabaseHealthCheck(ConductorDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var canConnect = await _dbContext.Database.CanConnectAsync(cancellationToken);

        return canConnect
            ? HealthCheckResult.Healthy("Database connection is healthy")
            : HealthCheckResult.Unhealthy("Unable to connect to the database");
    }
}
