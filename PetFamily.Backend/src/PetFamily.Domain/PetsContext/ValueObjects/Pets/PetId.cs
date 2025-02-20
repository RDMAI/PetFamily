namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetId : IComparable<PetId>
{
    public Guid Value { get; }

    //IComparable<Id>
    public int CompareTo(PetId? other) => Value.CompareTo(other?.Value);

    public static PetId GenerateNew() => new(Guid.NewGuid());
    public static PetId Empty() => new(Guid.Empty);
    public static PetId Create(Guid id) => new(id);

    protected PetId(Guid value)
    {
        Value = value;
    }
}
