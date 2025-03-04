using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Infrastructure.BackgroundServices;
using PetFamily.Infrastructure.Repositories;

namespace PetFamily.Infrastructure.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContextFactory<ApplicationDBContext>();
        services.AddScoped<IVolunteerRepository, VolunteerRepository>();

        services.AddHostedService<SoftDeleteCleanerBackgroundService>();

        return services;
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}
