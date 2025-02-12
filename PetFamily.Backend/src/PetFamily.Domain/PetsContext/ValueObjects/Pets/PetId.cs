namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetId : IComparable<PetId>
{
    public Guid Value { get; }

    //IComparable<PetId>
    public int CompareTo(PetId? other) => Value.CompareTo(other?.Value);
}
