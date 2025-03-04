using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Infrastructure.BackgroundServices;
using PetFamily.Infrastructure.Repositories;

namespace PetFamily.Infrastructure.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<ApplicationDBContext>();
        services.AddScoped<IVolunteerRepository, VolunteerRepository>();

        services.AddSoftDeleteCleaner(configuration);

        return services;
    }

    private static IServiceCollection AddSoftDeleteCleaner(this IServiceCollection services, IConfiguration configuration)
    {
        // getting configurations
        if (!float.TryParse(configuration["SoftDeleteCleaner:CheckPeriodHours"], out var checkPeriodHours))
            throw new ApplicationException("CheckPeriodHours is not configured");

        if (!float.TryParse(configuration["SoftDeleteCleaner:TimeToRestoreHours"], out var timeToRestore))
            throw new ApplicationException("TimeToRestoreHours is not configured");

        // adding options to services
        services.Configure<SoftDeleteCleanerOptions>(o =>
        {
            o.CheckPeriod = TimeSpan.FromHours(checkPeriodHours);
            o.TimeToRestore = TimeSpan.FromHours(timeToRestore);
        });

        // registering bg service with created options
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
