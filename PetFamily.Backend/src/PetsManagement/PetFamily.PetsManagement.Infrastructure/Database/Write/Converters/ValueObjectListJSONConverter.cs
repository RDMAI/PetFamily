using Microsoft.EntityFrameworkCore.ChangeTracking;
using PetFamily.Domain.Shared.Primitives;
using System.Text.Json;

namespace PetFamily.PetsManagement.Infrastructure.Database.Write.Converters;

public static class ValueObjectListJSONConverter
{
    public static string Serialize<T>(ValueObjectList<T> list)
    {
        return JsonSerializer.Serialize(list.Values, JsonSerializerOptions.Default);
    }

    public static ValueObjectList<T> Deserialize<T>(string json)
    {
        if (json == "{}") return new ValueObjectList<T>([]);  // default empty value

        var deserialized = JsonSerializer.Deserialize<T[]>(json, JsonSerializerOptions.Default);
        if (deserialized == null) return new ValueObjectList<T>([]);  // if value somehow is null

        return new ValueObjectList<T>(deserialized);
    }

    public static ValueComparer<ValueObjectList<T>> GetValueComparer<T>()
    {
        return new ValueComparer<ValueObjectList<T>>(
            (vl1, vl2) => vl1!.SequenceEqual(vl2!),
            vl => vl.Aggregate(0, (s, f) => HashCode.Combine(s, f!.GetHashCode())),
            vl => (ValueObjectList<T>)vl.ToList());
    }
}
