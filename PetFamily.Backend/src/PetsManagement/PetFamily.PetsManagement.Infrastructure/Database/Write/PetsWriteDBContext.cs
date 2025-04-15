using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetFamily.PetsManagement.Domain.Entities;

namespace PetFamily.PetsManagement.Infrastructure.Database.Write;

public class PetsWriteDBContext : DbContext
{
    private readonly string _connectionString;

    public DbSet<Volunteer> Volunteers => Set<Volunteer>();

    public PetsWriteDBContext(string connectionString)
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
            typeof(PetsWriteDBContext).Assembly,
            type => type.FullName?.Contains("Database.Write.Configurations") ?? false);

        base.OnModelCreating(modelBuilder);
    }

    private ILoggerFactory CreateLoggerFactory() =>
        LoggerFactory.Create(builder => { builder.AddConsole(); });
}
