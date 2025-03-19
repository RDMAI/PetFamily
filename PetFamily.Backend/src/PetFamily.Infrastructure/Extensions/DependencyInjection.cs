using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.Shared.Messaging;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Infrastructure.BackgroundServices;
using PetFamily.Infrastructure.EFHelpers;
using PetFamily.Infrastructure.MessageQueues;
using PetFamily.Infrastructure.Options;
using PetFamily.Infrastructure.Providers;
using PetFamily.Infrastructure.Repositories;

namespace PetFamily.Infrastructure.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // for bg services and ws connections
        services.AddDbContextFactory<ApplicationDBContext>();

        // scopped dbcontext = different context for each web request
        services.AddDbContext<ApplicationDBContext>();
        services.AddScoped<IUnitOfWork, EFUnitOfWork>();

        services.AddScoped<IVolunteerRepository, VolunteerRepository>();
        services.AddScoped<ISpeciesRepository, SpeciesRepository>();

        services.AddSingleton<IMessageQueue<IEnumerable<FileInfoDTO>>, InMemoryMessageQueue<IEnumerable<FileInfoDTO>>>();

        services.AddSoftDeleteCleaner(configuration);
        services.AddFileCleaner(configuration);

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

    private static IServiceCollection AddFileCleaner(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddHostedService<FileCleanerBackgroundService>();

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

    public static async Task CreateFileStorageStructure(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var fileProvider = scope.ServiceProvider.GetRequiredService<IFileProvider>();
        await fileProvider.CreateRequiredBuckets();
    }
}
