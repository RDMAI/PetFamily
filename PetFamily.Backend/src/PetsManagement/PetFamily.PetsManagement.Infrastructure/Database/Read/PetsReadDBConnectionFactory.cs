using Npgsql;
using PetFamily.Shared.Core.Abstractions;
using System.Data;

namespace PetFamily.PetsManagement.Infrastructure.Database.Read;

public class PetsReadDBConnectionFactory : IDBConnectionFactory
{
    private readonly string _connectionString;

    public PetsReadDBConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
