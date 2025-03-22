using Microsoft.Extensions.Configuration;
using Npgsql;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Infrastructure.DataBaseAccess.Write;
using System.Data;

namespace PetFamily.Infrastructure.DataBaseAccess.Read;

public class DapperConnectionFactory : IDBConnectionFactory
{
    private readonly IConfiguration _configuration;

    public DapperConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection Create()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString(WriteDBContext.DATABASE));
    }
}
