using PetFamily.Shared.Core.Database.Read.TypeHandlers;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Shared.Core.Database.Read;

public class DapperConfigurationHelper
{
    public static void Configure()
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        Dapper.SqlMapper.AddTypeHandler(new JsonTypeHandler<Requisites[]>());
        Dapper.SqlMapper.AddTypeHandler(new JsonTypeHandler<SocialNetwork[]>());
        Dapper.SqlMapper.AddTypeHandler(new JsonTypeHandler<Kernel.ValueObjects.File[]>());
        Dapper.SqlMapper.AddTypeHandler(new DateOnlyTypeHandler());
    }
}
