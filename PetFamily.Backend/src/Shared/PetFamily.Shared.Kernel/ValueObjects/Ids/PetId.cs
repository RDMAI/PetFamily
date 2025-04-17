namespace PetFamily.Shared.Kernel.ValueObjects.Ids;

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

    // EF Core
    private PetId() { }
}
