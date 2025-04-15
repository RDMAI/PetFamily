using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.SpeciesManagement.Domain.Entities;

namespace PetFamily.SpeciesManagement.Infrastructure.Database.Write;

public class SpeciesWriteDBContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<Species> Species => Set<Species>();

    public SpeciesWriteDBContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.UseSnakeCaseNamingConvention();
        optionsBuilder.UseLoggerFactory(CreateLoggerFactory());

        base.OnConfiguring(optionsBuilder);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(SpeciesWriteDBContext).Assembly,
            type => type.FullName?.Contains("Database.Write.Configurations") ?? false);

        base.OnModelCreating(modelBuilder);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}
