namespace PetFamily.Domain.PetsContext.ValueObjects.Volunteers;

public record VolunteerId : IComparable<VolunteerId>
{
    public Guid Value { get; }

    //IComparable<PetId>
    public int CompareTo(VolunteerId? other) => Value.CompareTo(other?.Value);
}
