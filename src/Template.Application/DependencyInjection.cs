namespace Template.Application;

using FluentValidation;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wolverine;
using Wolverine.FluentValidation;

public static class DependencyInjection
{
    public static IHostBuilder AddApplication(
        this IHostBuilder hostBuilder,
        Action<WolverineOptions>? configureOptions = null)
    {
        hostBuilder.UseWolverine(opts =>
        {
            // Solo durability mode: local queues are enabled (so cascading messages from
            // command handlers actually reach their handlers) but no external broker or
            // message storage is required. Switch to DurabilityMode.MediatorOnly if you
            // only want pure request/reply and can shed the durability runtime overhead.
            opts.Durability.Mode = DurabilityMode.Solo;
            opts.Discovery.IncludeAssembly(typeof(DependencyInjection).Assembly);
            opts.UseFluentValidation();
            configureOptions?.Invoke(opts);
        });
        return hostBuilder;
    }

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        TypeAdapterConfig typeAdapterConfig = TypeAdapterConfig.GlobalSettings;
        typeAdapterConfig.Scan(typeof(DependencyInjection).Assembly);
        services.AddSingleton(typeAdapterConfig);

        services.AddValidatorsFromAssembly(
            typeof(DependencyInjection).Assembly,
            Microsoft.Extensions.DependencyInjection.ServiceLifetime.Singleton);

        return services;
    }
}