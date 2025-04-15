using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.SpeciesManagement.Infrastructure.Extensions;
using PetFamily.Shared.Core.Extensions;

namespace PetFamily.SpeciesManagement.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddSpeciesManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddInfrastructure(configuration)
            .AddApplication(
                typeInAssembly: typeof(PetFamily.SpeciesManagement.Presentation.DependencyInjection));


        return services;
    }
}
