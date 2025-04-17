using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Extensions;
using PetFamily.PetsManagement.Infrastructure.Extensions;

namespace PetFamily.PetsManagement.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddPetsManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddInfrastructure(configuration)
            .AddApplication();


        return services;
    }
}
