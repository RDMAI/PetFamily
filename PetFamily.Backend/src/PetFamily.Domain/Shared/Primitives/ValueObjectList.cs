using CSharpFunctionalExtensions;
using System.Collections;

namespace PetFamily.Domain.Shared.Primitives;

public class ValueObjectList<T> : IReadOnlyList<T>
{
    private readonly List<T> _values = [];
    public IReadOnlyList<T> Values => _values;
    public T this[int index] => _values[index];
    public int Count => _values.Count;

    // EF Core
    private ValueObjectList() { }

    public ValueObjectList(IEnumerable<T> list)
    {
        _values = new List<T>(list);
    }

    public IEnumerator<T> GetEnumerator() =>
        _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        _values.GetEnumerator();

    public static implicit operator ValueObjectList<T>(List<T> list) =>
        new(list);

    public static implicit operator List<T>(ValueObjectList<T> list) =>
        list._values.ToList();
}
