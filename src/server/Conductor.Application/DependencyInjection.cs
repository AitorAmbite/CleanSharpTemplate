namespace Conductor.Application;

using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        return services;
    }
}
