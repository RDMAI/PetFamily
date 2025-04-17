using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SpeciesManagement.Application.Exstensions;
using PetFamily.SpeciesManagement.Contracts;
using PetFamily.SpeciesManagement.Infrastructure.Extensions;

namespace PetFamily.SpeciesManagement.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddSpeciesManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddInfrastructure(configuration)
            .AddApplication();

        services.AddScoped<ISpeciesContract, SpeciesContract>();

        return services;
    }
}
