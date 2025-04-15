using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.Shared.Core.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        Type typeInAssembly)
    {
        var assembly = typeInAssembly.Assembly;

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
