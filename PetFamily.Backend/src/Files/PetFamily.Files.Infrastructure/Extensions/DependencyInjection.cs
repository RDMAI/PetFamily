using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using PetFamily.Files.Application;
using PetFamily.Files.Infrastructure.BackgroundServices;
using PetFamily.Files.Infrastructure.Minio;
using PetFamily.Shared.Core.Files;
using PetFamily.Shared.Core.Messaging;

namespace PetFamily.Files.Infrastructure.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddFilesInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddSingleton<IMessageQueue<IEnumerable<FileInfoDTO>>, InMemoryMessageQueue<IEnumerable<FileInfoDTO>>>()
            .AddConfiguredMinio(configuration)
            .AddFileCleaner(configuration);

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

    public static async Task CreateFileStorageStructure(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var fileProvider = scope.ServiceProvider.GetRequiredService<IFileProvider>();
        await fileProvider.CreateRequiredBuckets();
    }
}
