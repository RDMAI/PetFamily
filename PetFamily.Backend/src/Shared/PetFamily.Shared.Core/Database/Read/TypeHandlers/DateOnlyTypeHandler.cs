using Dapper;
using System.Data;

namespace PetFamily.Shared.Core.Database.Read.TypeHandlers;
public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value)
        => DateOnly.FromDateTime((DateTime)value);

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
        => parameter.Value = value.ToDateTime(new TimeOnly(0, 0));
}
