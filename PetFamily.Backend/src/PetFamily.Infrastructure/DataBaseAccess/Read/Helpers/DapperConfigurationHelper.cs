using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Infrastructure.DataBaseAccess.Read.Helpers;

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
