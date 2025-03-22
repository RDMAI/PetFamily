using PetFamily.Domain.Shared.ValueObjects;
using System.Text.Json.Serialization;

namespace PetFamily.Infrastructure.DataBaseAccess.Read;

public class DapperConfigurationHelper
{
    public static void Configure()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        Dapper.SqlMapper.AddTypeHandler(new JsonTypeHandler<Requisites[]>());
        Dapper.SqlMapper.AddTypeHandler(new JsonTypeHandler<SocialNetwork[]>());
        Dapper.SqlMapper.AddTypeHandler(new JsonTypeHandler<FileVO[]>());
    }
}
