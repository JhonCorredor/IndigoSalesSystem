using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Shared.Utilities.Mappers;

namespace Shared.Utilities.Extensions;

/// <summary>
/// Extension methods for service collection configuration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all mappers from the specified assembly.
    /// </summary>
    public static IServiceCollection AddMappersFromAssembly(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var mapperInterfaceType = typeof(IMapper<,>);

        var mapperTypes = assembly.GetTypes()
            .Where(t => t is { IsAbstract: false, IsInterface: false })
            .SelectMany(t => t.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == mapperInterfaceType)
                .Select(i => new { Implementation = t, Interface = i }));

        foreach (var mapper in mapperTypes)
        {
            var descriptor = new ServiceDescriptor(mapper.Interface, mapper.Implementation, lifetime);
            services.Add(descriptor);
        }

        return services;
    }

    /// <summary>
    /// Registers a specific mapper.
    /// </summary>
    public static IServiceCollection AddMapper<TMapper, TSource, TDestination>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TMapper : class, IMapper<TSource, TDestination>
    {
        var descriptor = new ServiceDescriptor(
            typeof(IMapper<TSource, TDestination>),
            typeof(TMapper),
            lifetime);

        services.Add(descriptor);
        return services;
    }
}
