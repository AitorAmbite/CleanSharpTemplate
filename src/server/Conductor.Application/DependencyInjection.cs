namespace Conductor.Application;

using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Wolverine;

public static class DependencyInjection
{
    public static IHostBuilder AddApplication(this IHostBuilder hostBuilder)
    {
        hostBuilder.UseWolverine();
        return hostBuilder;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMapster();
        services.AddQuartz(options =>
        {
            options.UsePersistentStore(x =>
            {
                x.UsePostgres(sqlServerOptions =>
                {
                    sqlServerOptions.ConnectionString = "Host=localhost;Port=5432;Database=conductor;Username=postgres;Password=postgres";
                });
            });
        });
        return services;
    }
}
