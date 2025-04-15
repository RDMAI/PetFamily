namespace PetFamily.Shared.Kernel.ValueObjects.Ids;

public record VolunteerId : IComparable<VolunteerId>
{
    public Guid Value { get; }

    //IComparable<Id>
    public int CompareTo(VolunteerId? other) => Value.CompareTo(other?.Value);

    public static VolunteerId GenerateNew() => new(Guid.NewGuid());
    public static VolunteerId Empty() => new(Guid.Empty);
    public static VolunteerId Create(Guid id) => new(id);

    protected VolunteerId(Guid value)
    {
        Value = value;
    }
}
