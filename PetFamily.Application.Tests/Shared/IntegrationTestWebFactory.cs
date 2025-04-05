using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using Npgsql;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Infrastructure.DataBaseAccess.Read;
using PetFamily.Infrastructure.DataBaseAccess.Write;
using Respawn;
using Testcontainers.PostgreSql;

namespace PetFamily.Application.IntegrationTests.Shared;
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
    private Mock<IFileProvider> _fileProviderMock = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(ReconfigureServices);
    }

    protected virtual void ReconfigureServices(IServiceCollection services)
    {
        services.RemoveAll<IDBConnectionFactory>();
        services.AddSingleton<IDBConnectionFactory, DapperConnectionFactory>(_ =>
            new DapperConnectionFactory(_dbContainer.GetConnectionString()));

        services.RemoveAll<IDbContextFactory<WriteDBContext>>();
        services.AddSingleton<IDbContextFactory<WriteDBContext>>(_ =>
            new WriteDBContextFactory(_dbContainer.GetConnectionString()));

        services.RemoveAll<WriteDBContext>();
        services.AddScoped(_ =>
            new WriteDBContext(_dbContainer.GetConnectionString()));

        services.RemoveAll<IFileProvider>();
        services.AddScoped(_ => _fileProviderMock.Object);
    }

    public void SetupSuccessFileProviderMock()
    {
        _fileProviderMock
            .Setup(f => f.UploadFilesAsync(It.IsAny<IEnumerable<FileData>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<ErrorList>());

        _fileProviderMock
            .Setup(f => f.DeleteFilesAsync(It.IsAny<IEnumerable<FileInfoDTO>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Success<ErrorList>());
    }

    public void SetupFailureFileProviderMock()
    {
        _fileProviderMock
            .Setup(f => f.UploadFilesAsync(It.IsAny<IEnumerable<FileData>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Failure<ErrorList>(ErrorHelper.Files.UploadFailure()));

        _fileProviderMock
            .Setup(f => f.DeleteFilesAsync(It.IsAny<IEnumerable<FileInfoDTO>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(UnitResult.Failure<ErrorList>(ErrorHelper.Files.DeleteFailure()));
    }

    public async Task InitializeRespawner()
    {
        _dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());

        await _dbConnection.OpenAsync();

        _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres
        });
    }

    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        var factory = Services.GetRequiredService<IDbContextFactory<WriteDBContext>>();
        using var dbContext = factory.CreateDbContext();
        await dbContext.Database.EnsureCreatedAsync();

        await InitializeRespawner();
    }

    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }
}
