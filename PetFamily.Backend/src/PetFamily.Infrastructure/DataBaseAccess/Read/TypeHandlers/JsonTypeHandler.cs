﻿using Dapper;
using System.Data;
using System.Text.Json;

namespace PetFamily.Infrastructure.DataBaseAccess.Read.TypeHandlers;
public class JsonTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    public override T Parse(object value)
    {
        string json = (string)value;
        return JsonSerializer.Deserialize<T>(json);
    }

    public override void SetValue(IDbDataParameter parameter, T value)
    {
        parameter.Value = JsonSerializer.Serialize(value);
    }
}
