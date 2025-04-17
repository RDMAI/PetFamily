using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Files.Contracts;
using PetFamily.Files.Infrastructure.Extensions;

namespace PetFamily.Files.Presentation;

public static class DependencyInjection
{
    public static IServiceCollection AddFilesManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddFilesInfrastructure(configuration);

        services.AddScoped<IFileContract, FileContract>();

        return services;
    }
}
