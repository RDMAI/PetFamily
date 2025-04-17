using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PetFamily.Shared.Core;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Database.Read;
using PetFamily.SpeciesManagement.Application.Interfaces;
using PetFamily.SpeciesManagement.Infrastructure.Database;
using PetFamily.SpeciesManagement.Infrastructure.Database.Read;
using PetFamily.SpeciesManagement.Infrastructure.Database.Write;
using PetFamily.SpeciesManagement.Infrastructure.Database.Write.Repositories;

namespace PetFamily.SpeciesManagement.Infrastructure.Extensions;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDatabaseAccess(configuration);

        return services;
    }

    private static IServiceCollection AddDatabaseAccess(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConnectionString = configuration.GetConnectionString(DBConstants.DATABASE);

        // dapper for reads
        DapperConfigurationHelper.Configure();

        var key = DependencyHelper.DependencyKey.Species;
        services.AddKeyedSingleton<IDBConnectionFactory, SpeciesReadDBConnectionFactory>(
            serviceKey: key,
            implementationFactory: (_, key) => new SpeciesReadDBConnectionFactory(dbConnectionString));

        // for bg services and ws connections
        services.AddSingleton<IDbContextFactory<SpeciesWriteDBContext>>(_ =>
            new SpeciesWriteDBContextFactory(dbConnectionString));

        // scopped dbcontext = different context for each web request
        services.AddScoped(_ => new SpeciesWriteDBContext(dbConnectionString));
        services.AddKeyedScoped<IUnitOfWork, SpeciesUnitOfWork>(
            serviceKey: key);

        services.AddScoped<ISpeciesAggregateRepository, SpeciesRepository>();

        return services;
    }

    public static async Task ApplyMigrations(this IHost host)
    {
        await using var scope = host.Services.CreateAsyncScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<SpeciesWriteDBContext>();
        await dbContext.Database.MigrateAsync();
    }
}
