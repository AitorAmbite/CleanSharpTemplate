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
            opts.Durability.Mode = DurabilityMode.MediatorOnly;
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