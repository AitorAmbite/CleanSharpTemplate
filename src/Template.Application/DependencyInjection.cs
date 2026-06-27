namespace Template.Application;

using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;

public static class DependencyInjection
{
    public static IHostBuilder AddApplication(
        this IHostBuilder hostBuilder,
        Action<WolverineOptions>? configureOptions = null)
    {
        hostBuilder.UseWolverine(opts =>
        {
            opts.Durability.Mode = DurabilityMode.MediatorOnly;
            opts.Discovery.IncludeAssembly(typeof(DependencyInjection).Assembly);
            configureOptions?.Invoke(opts);
        });
        return hostBuilder;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMapster();

        return services;
    }
}
