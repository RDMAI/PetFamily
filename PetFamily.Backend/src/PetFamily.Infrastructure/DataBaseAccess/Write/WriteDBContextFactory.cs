using Microsoft.EntityFrameworkCore;

namespace PetFamily.Infrastructure.DataBaseAccess.Write;

public class WriteDBContextFactory : IDbContextFactory<WriteDBContext>
{
    private readonly string _connectionString;
    public WriteDBContextFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public WriteDBContext CreateDbContext()
    {
        return new WriteDBContext(_connectionString);
    }
}
