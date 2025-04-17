using Microsoft.EntityFrameworkCore;

namespace PetFamily.PetsManagement.Infrastructure.Database.Write;

public class PetsWriteDBContextFactory : IDbContextFactory<PetsWriteDBContext>
{
    private readonly string _connectionString;
    public PetsWriteDBContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public PetsWriteDBContext CreateDbContext()
    {
        return new PetsWriteDBContext(_connectionString);
    }
}
