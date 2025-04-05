using Dapper;
using System.Data;
using System.Text.Json;

namespace PetFamily.Infrastructure.DataBaseAccess.Read.TypeHandlers;
public class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value)
        => DateOnly.FromDateTime((DateTime)value);

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
        => parameter.Value = value.ToDateTime(new TimeOnly(0, 0));
}
