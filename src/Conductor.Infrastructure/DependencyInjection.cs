namespace Conductor.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Conductor.Application.Configuration;
using Conductor.Domain;
using Conductor.Domain.Repositories;
using Conductor.Infrastructure.Persistence;
using Conductor.Infrastructure.Persistence.Interceptors;
using Conductor.Infrastructure.Persistence.Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseConfig>(configuration.GetSection("DatabaseConfig"));

        services.AddDbContext<ConductorDbContext>((serviceProvider, options) =>
        {
            var databaseConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfig>>().Value;

                options.UseNpgsql(
                    databaseConfig.ConnectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.MigrationsAssembly(typeof(ConductorDbContext).Assembly.FullName);
                    });

                options.AddInterceptors(new EntityAuditInterceptor());
        });

        services.AddScoped<IUnitOfWork, EfUnitOfWork>();
        services.AddScoped<ITodoRepository, TodoRepository>();

        return services;
    }
}
