using Npgsql;
using PetFamily.Shared.Core.Abstractions;
using System.Data;

namespace PetFamily.SpeciesManagement.Infrastructure.Database.Read;

public class SpeciesReadDBConnectionFactory : IDBConnectionFactory
{
    private readonly string _connectionString;

    public SpeciesReadDBConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
