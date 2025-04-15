using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Infrastructure.Extensions;
using PetFamily.Shared.Core.Extensions;

namespace PetFamily.PetsManagement.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPetsManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddInfrastructure(configuration)
            .AddApplication(
                typeInAssembly: typeof(PetFamily.PetsManagement.Presentation.DependencyInjection));


        return services;
    }
}
