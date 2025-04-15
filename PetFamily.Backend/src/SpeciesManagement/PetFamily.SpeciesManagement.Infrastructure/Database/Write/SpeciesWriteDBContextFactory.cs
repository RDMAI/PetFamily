using Microsoft.EntityFrameworkCore;

namespace PetFamily.SpeciesManagement.Infrastructure.Database.Write;

public class SpeciesWriteDBContextFactory : IDbContextFactory<SpeciesWriteDBContext>
{
    private readonly string _connectionString;
    public SpeciesWriteDBContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public SpeciesWriteDBContext CreateDbContext()
    {
        return new SpeciesWriteDBContext(_connectionString);
    }
}
