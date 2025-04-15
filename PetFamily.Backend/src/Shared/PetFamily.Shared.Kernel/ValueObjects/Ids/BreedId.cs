namespace PetFamily.Shared.Kernel.ValueObjects.Ids;
public record BreedId : IComparable<BreedId>
{
    public Guid Value { get; }

    //IComparable<Id>
    public int CompareTo(BreedId? other) => Value.CompareTo(other?.Value);

    public static BreedId GenerateNew() => new(Guid.NewGuid());
    public static BreedId Empty() => new(Guid.Empty);
    public static BreedId Create(Guid id) => new(id);

    protected BreedId(Guid value)
    {
        Value = value;
    }

    // EF Core
    private BreedId() { }
}
