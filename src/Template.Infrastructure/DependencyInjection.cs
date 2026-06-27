namespace Template.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Template.Application.Configuration;
using Template.Domain;
using Template.Domain.Repositories;
using Template.Infrastructure.Persistence;
using Template.Infrastructure.Persistence.Interceptors;
using Template.Infrastructure.Persistence.Repositories;

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
