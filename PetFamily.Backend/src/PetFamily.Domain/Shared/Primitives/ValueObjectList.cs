using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared.ValueObjects;
using System.Collections;

namespace PetFamily.Domain.Shared.Primitives;

public class ValueObjectList<T> : IReadOnlyList<T>
{
    private readonly List<T> _values = [];
    public IReadOnlyList<T> Values => _values;
    public T this[int index] => Values[index];
    public int Count => Values.Count;

    // EF Core
    private ValueObjectList() { }

    public ValueObjectList(IEnumerable<T> list)
    {
        _values = new List<T>(list);
    }

    public IEnumerator<T> GetEnumerator() =>
        Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        Values.GetEnumerator();

    public static implicit operator ValueObjectList<T>(List<T> list) =>
        new(list);

    public static implicit operator List<T>(ValueObjectList<T> list) =>
        list.Values.ToList();
}
