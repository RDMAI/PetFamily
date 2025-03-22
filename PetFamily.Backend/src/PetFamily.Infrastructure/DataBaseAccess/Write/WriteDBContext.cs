using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.SpeciesContext.Entities;

namespace PetFamily.Infrastructure.DataBaseAccess.Write;

public class WriteDBContext : DbContext
{
    private readonly IConfiguration _configuration;
    // PostgresDB to connect to docker container, PostgresDBLocal for local database
    public const string DATABASE = "PostgresDB";

    public DbSet<Volunteer> Volunteers => Set<Volunteer>();
    public DbSet<Species> Species => Set<Species>();

    public WriteDBContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString(DATABASE));  // gets connection string from secrets
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(WriteDBContext).Assembly,
            type => type.FullName?.Contains("DataBaseAccess.Write.Configurations") ?? false);

        base.OnModelCreating(modelBuilder);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}
