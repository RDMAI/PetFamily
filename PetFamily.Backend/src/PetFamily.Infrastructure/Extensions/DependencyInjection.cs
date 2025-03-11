using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Infrastructure.BackgroundServices;
using PetFamily.Infrastructure.Options;
using PetFamily.Infrastructure.Providers;
using PetFamily.Infrastructure.Repositories;

namespace PetFamily.Infrastructure.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContextFactory<ApplicationDBContext>();
        services.AddScoped<IVolunteerRepository, VolunteerRepository>();

        services.AddSoftDeleteCleaner(configuration);

        services.AddConfiguredMinio(configuration);

        return services;
    }

    private static IServiceCollection AddSoftDeleteCleaner(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var configs = configuration.GetSection("SoftDeleteCleaner");

        services.Configure<SoftDeleteCleanerOptions>(configs);
        services.AddHostedService<SoftDeleteCleanerBackgroundService>();

        return services;
    }

    private static IServiceCollection AddConfiguredMinio(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var configs = configuration.GetSection("Minio");

        //services.Configure<MinioOptions>(configs);
        var options = configs.Get<MinioOptions>()
                ?? throw new ApplicationException("Minio is not configured");

        services.AddMinio(client =>
        {
            client.WithEndpoint(options.Endpoint);
            client.WithCredentials(options.Login, options.Password);
            client.WithSSL(options.WithSSL);

            client.Build();
        });

        services.AddScoped<IFileProvider, MinioProvider>();

        return services;
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}
