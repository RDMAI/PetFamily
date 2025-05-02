using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Npgsql;
using PetFamily.Accounts.Infrastructure.Identity;
using PetFamily.Accounts.Infrastructure.Identity.Options;
using PetFamily.Files.Contracts;
using PetFamily.Files.Contracts.Requests;
using PetFamily.PetsManagement.Infrastructure.Database.Read;
using PetFamily.PetsManagement.Infrastructure.Database.Write;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.SpeciesManagement.Infrastructure.Database.Read;
using PetFamily.SpeciesManagement.Infrastructure.Database.Write;
using Respawn;
using Testcontainers.PostgreSql;
using Xunit;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.Tests.Shared;
public class IntegrationTestWebFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithImage("postgres")
        .WithDatabase("pet_family_tests")
        .WithUsername("postgresUser")
        .WithPassword("postgresPassword")
        .Build();

    private Respawner _respawner = default;
    private NpgsqlConnection _dbConnection = default;
    private Mock<IFileContract> _fileAPIMock = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ReconfigureServices);
    }

    protected virtual void ReconfigureServices(IServiceCollection services)
    {
        var connectionString = _dbContainer.GetConnectionString();

        ReconfigureServicesAccounts(services, connectionString);
        ReconfigureServicesPetsManagement(services, connectionString);
        ReconfigureServicesSpeciesManagement(services, connectionString);

        services.RemoveAll<IFileContract>();
        services.AddScoped(_ => _fileAPIMock.Object);
    }

    public void ReconfigureServicesAccounts(
        IServiceCollection services,
        string connectionString)
    {
        services.RemoveAll<AccountDBContext>();
        services.AddScoped(_ => new AccountDBContext(connectionString));

        Dictionary<string, string> configForAdmin = new()
        {
            {"Admin:Username", "admin"},
            {"Admin:Email", "admin@gmail.com"},
            {"Admin:Password", "string123_A"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configForAdmin)
            .Build();

        services.RemoveAll<AdminOptions>();
        services.Configure<AdminOptions>(configuration.GetSection(AdminOptions.ADMIN));
        services.AddOptions<AdminOptions>();
    }

    public void ReconfigureServicesPetsManagement(
        IServiceCollection services,
        string connectionString)
    {
        var key = DependencyKey.Pets;

        services.RemoveAllKeyed<IDBConnectionFactory>(key);
        services.AddKeyedSingleton<IDBConnectionFactory, PetsReadDBConnectionFactory>(
            serviceKey: key,
            implementationFactory: (_, key) => new PetsReadDBConnectionFactory(connectionString));

        services.RemoveAll<IDbContextFactory<PetsWriteDBContext>>();
        services.AddSingleton<IDbContextFactory<PetsWriteDBContext>>(_ =>
            new PetsWriteDBContextFactory(connectionString));

        services.RemoveAll<PetsWriteDBContext>();
        services.AddScoped(_ => new PetsWriteDBContext(connectionString));
    }

    public void ReconfigureServicesSpeciesManagement(
        IServiceCollection services,
        string connectionString)
    {
        var key = DependencyKey.Species;

        services.RemoveAllKeyed<IDBConnectionFactory>(key);
        services.AddKeyedSingleton<IDBConnectionFactory, SpeciesReadDBConnectionFactory>(
            serviceKey: key,
            implementationFactory: (_, key) => new SpeciesReadDBConnectionFactory(connectionString));

        services.RemoveAll<IDbContextFactory<SpeciesWriteDBContext>>();
        services.AddSingleton<IDbContextFactory<SpeciesWriteDBContext>>(_ =>
            new SpeciesWriteDBContextFactory(connectionString));

        services.RemoveAll<SpeciesWriteDBContext>();
        services.AddScoped(_ => new SpeciesWriteDBContext(connectionString));
    }

    public void SetupSuccessFileProviderMock()
    {
        _fileAPIMock
            .Setup(f => f.UploadFilesAsync(It.IsAny<UploadFilesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<ErrorList>());

        _fileAPIMock
            .Setup(f => f.DeleteFilesAsync(It.IsAny<DeleteFilesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<ErrorList>());
    }

    public void SetupFailureFileProviderMock()
    {
        _fileAPIMock
            .Setup(f => f.UploadFilesAsync(It.IsAny<UploadFilesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Failure<ErrorList>(ErrorHelper.Files.UploadFailure()));

        _fileAPIMock
            .Setup(f => f.DeleteFilesAsync(It.IsAny<DeleteFilesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Failure<ErrorList>(ErrorHelper.Files.DeleteFailure()));
    }

    public async Task InitializeRespawner()
    {
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());

        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = [ "pets_management", "species_management", "accounts"]
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var petsFactory = Services.GetRequiredService<IDbContextFactory<PetsWriteDBContext>>();
        using var petsContext = petsFactory.CreateDbContext();
        await petsContext.Database.EnsureCreatedAsync();

        var speciesFactory = Services.GetRequiredService<IDbContextFactory<SpeciesWriteDBContext>>();
        using var speciesContext = speciesFactory.CreateDbContext();
        await speciesContext.Database.EnsureCreatedAsync();

        using var scope = Services.CreateScope();
        var accountContext = scope.ServiceProvider.GetRequiredService<AccountDBContext>();
        await accountContext.Database.EnsureCreatedAsync();

        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
