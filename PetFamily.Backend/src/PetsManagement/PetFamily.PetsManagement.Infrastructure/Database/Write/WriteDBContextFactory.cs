using Microsoft.EntityFrameworkCore;

namespace PetFamily.PetsManagement.Infrastructure.Database.Write;

public class WriteDBContextFactory : IDbContextFactory<PetsWriteDBContext>
{
    private readonly string _connectionString;
    public WriteDBContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public PetsWriteDBContext CreateDbContext()
    {
        return new PetsWriteDBContext(_connectionString);
    }
}
