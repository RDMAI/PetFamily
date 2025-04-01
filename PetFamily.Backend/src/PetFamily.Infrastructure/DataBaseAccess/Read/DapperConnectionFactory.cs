using Microsoft.Extensions.Configuration;
using Npgsql;
using PetFamily.Application.Shared.Interfaces;
using System.Data;

namespace PetFamily.Infrastructure.DataBaseAccess.Read;

public class DapperConnectionFactory : IDBConnectionFactory
{
    private readonly string _connectionString;

    public DapperConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_connectionString);
    }
}
