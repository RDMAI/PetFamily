using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Shared.Core.Abstractions;
using System.Reflection;

namespace PetFamily.Shared.Core;

public static class DependencyHelper
{
    public enum DependencyKey
    {
        Pets = 1,
        Species = 2
    }

    public static IServiceCollection AddApplicationFromAssembly(
        IServiceCollection services,
        Assembly assembly)
    {
        // add commands from assembly
        services.Scan(scan => scan
            .FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableToAny(typeof(ICommandHandler<,>), typeof(ICommandHandler<>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        // add queries from assembly
        services.Scan(scan => scan
            .FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
            .AsSelfWithInterfaces()
            .WithScopedLifetime());

        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
