using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.PetsManagement.Infrastructure.BackgroundServices;
using PetFamily.PetsManagement.Infrastructure.Database;
using PetFamily.PetsManagement.Infrastructure.Database.Read;
using PetFamily.PetsManagement.Infrastructure.Database.Write;
using PetFamily.PetsManagement.Infrastructure.Database.Write.Repositories;
using PetFamily.PetsManagement.Infrastructure.Options;
using PetFamily.Shared.Core;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Database.Read;

namespace PetFamily.PetsManagement.Infrastructure.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabaseAccess(configuration)
            .AddSoftDeleteCleaner(configuration);

        return services;
    }

    private static IServiceCollection AddDatabaseAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString(DBConstants.DATABASE);

        // dapper for reads
        DapperConfigurationHelper.Configure();

        var key = DependencyHelper.DependencyKey.Pets;
        services.AddKeyedSingleton<IDBConnectionFactory, PetsReadDBConnectionFactory>(
            serviceKey: key,
            implementationFactory: (_, key) => new PetsReadDBConnectionFactory(dbConnectionString));

        // for bg services and ws connections
        services.AddSingleton<IDbContextFactory<PetsWriteDBContext>>(_ =>
            new PetsWriteDBContextFactory(dbConnectionString));

        // scopped dbcontext = different context for each web request
        services.AddScoped(_ => new PetsWriteDBContext(dbConnectionString));
        services.AddKeyedScoped<IUnitOfWork, PetsUnitOfWork>(
            serviceKey: key);

        services.AddScoped<IVolunteerAggregateRepository, VolunteerRepository>();

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

    public static async Task ApplyMigrations(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<PetsWriteDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}
