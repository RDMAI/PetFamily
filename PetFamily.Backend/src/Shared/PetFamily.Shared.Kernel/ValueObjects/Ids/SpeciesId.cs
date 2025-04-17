namespace PetFamily.Shared.Kernel.ValueObjects.Ids;

public record SpeciesId : IComparable<SpeciesId>
{
    public Guid Value { get; }

    //IComparable<Id>
    public int CompareTo(SpeciesId? other) => Value.CompareTo(other?.Value);

    public static SpeciesId GenerateNew() => new(Guid.NewGuid());
    public static SpeciesId Empty() => new(Guid.Empty);
    public static SpeciesId Create(Guid id) => new(id);

    protected SpeciesId(Guid value)
    {
        Value = value;
    }

    // EF Core
    private SpeciesId() { }
}
