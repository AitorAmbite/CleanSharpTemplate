namespace Conductor.Application;

using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Wolverine;
using Conductor.Application.Configuration;

public static class DependencyInjection
{
    public static IHostBuilder AddApplication(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseWolverine(opts =>
        {
            opts.Durability.Mode = DurabilityMode.MediatorOnly;
            opts.Discovery.IncludeAssembly(typeof(DependencyInjection).Assembly);
        });
        return hostBuilder;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMapster();

        var databaseConfig = configuration.GetSection("DatabaseConfig").Get<DatabaseConfig>()
            ?? throw new InvalidOperationException("DatabaseConfig section is missing.");

        services.AddQuartz(options =>
        {
            options.UsePersistentStore(x =>
            {
                x.UseProperties = true;
                x.UseSystemTextJsonSerializer();

                switch (databaseConfig.Type)
                {
                    case DatabaseType.Sqlite:
                        x.UseSQLite(sqliteOptions =>
                        {
                            sqliteOptions.ConnectionString = databaseConfig.ConnectionString;
                        });
                        break;
                    case DatabaseType.Postgres:
                        x.UsePostgres(postgresOptions =>
                        {
                            postgresOptions.ConnectionString = databaseConfig.ConnectionString;
                        });
                        break;
                    default:
                        throw new InvalidOperationException($"Database type '{databaseConfig.Type}' is not supported for Quartz.");
                }
            });
        });

        return services;
    }
}
