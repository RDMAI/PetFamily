//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Minio;
//using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
//using PetFamily.Application.SpeciesManagement.Interfaces;
//using PetFamily.Infrastructure.DataBaseAccess.Write;
//using PetFamily.Shared.Core.Abstractions;
//using PetFamily.Shared.Core.BackgroundServices;
//using PetFamily.Shared.Core.Files;
//using PetFamily.Shared.Core.Infrastructure.DataBaseAccess;
//using PetFamily.Shared.Core.Infrastructure.DataBaseAccess.Read;
//using PetFamily.Shared.Core.Infrastructure.DataBaseAccess.Read.Helpers;
//using PetFamily.Shared.Core.Infrastructure.DataBaseAccess.Write;
//using PetFamily.Shared.Core.Infrastructure.DataBaseAccess.Write.Repositories;
//using PetFamily.Shared.Core.Infrastructure.Options;
//using PetFamily.Shared.Core.Infrastructure.Providers;
//using PetFamily.Shared.Core.Messaging;

//namespace PetFamily.Shared.Core.Unsorted_Infrastructure.Extensions;
//public static class DependencyInjection
//{
//    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
//    {
//        services.AddDatabaseAccess(configuration);

//        return services;
//    }

//    private static IServiceCollection AddDatabaseAccess(this IServiceCollection services, IConfiguration configuration)
//    {
//        var dbConnectionString = configuration.GetConnectionString(DBConstants.DATABASE);

//        // dapper for reads
//        DapperConfigurationHelper.Configure();
//        services.AddSingleton<IDBConnectionFactory, DapperConnectionFactory>(_ =>
//            new DapperConnectionFactory(dbConnectionString));

//        // for bg services and ws connections
//        services.AddSingleton<IDbContextFactory<WriteDBContext>>(_ =>
//            new WriteDBContextFactory(dbConnectionString));

//        // scopped dbcontext = different context for each web request
//        services.AddScoped(_ => new WriteDBContext(dbConnectionString));
//        services.AddScoped<IUnitOfWork, EFUnitOfWork>();

//        services.AddScoped<IVolunteerAggregateRepository, VolunteerRepository>();
//        services.AddScoped<ISpeciesAggregateRepository, SpeciesRepository>();

//        return services;
//    }

//    private static IServiceCollection AddSoftDeleteCleaner(
//        this IServiceCollection services,
//        IConfiguration configuration)
//    {
//        var configs = configuration.GetSection("SoftDeleteCleaner");

//        services.Configure<SoftDeleteCleanerOptions>(configs);
//        services.AddHostedService<SoftDeleteCleanerBackgroundService>();

//        return services;
//    }

//    public static async Task ApplyMigrations(this IHost host)
//    {
//        await using var scope = host.Services.CreateAsyncScope();

//        var dbContext = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
//        await dbContext.Database.MigrateAsync();
//    }
//}
