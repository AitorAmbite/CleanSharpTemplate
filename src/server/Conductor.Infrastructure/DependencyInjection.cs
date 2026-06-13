namespace Conductor.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Conductor.Application.Configuration;
using Conductor.Domain;
using Conductor.Infrastructure.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConfig"));

        services.AddDbContext<ConductorDbContext>((serviceProvider, options) =>
        {
            var databaseConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfig>>().Value;

            switch (databaseConfig.Type)
            {
                case DatabaseType.Sqlite:
                    options.UseSqlite(databaseConfig.ConnectionString);
                    break;
                case DatabaseType.Postgres:
                    {
                        options.UseNpgsql(
                            databaseConfig.ConnectionString,
                            npgsqlOptions =>
                            {
                                npgsqlOptions.MigrationsAssembly(typeof(ConductorDbContext).Assembly.FullName);
                            });
                        break;
                    }
                default:
                    throw new InvalidOperationException($"Database type '{databaseConfig.Type}' is not supported.");
            }
        });

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ConductorDbContext>());
        services.AddScoped<ICronExpressionValidator, Services.CronExpressionValidator>();

        return services;
    }
}
