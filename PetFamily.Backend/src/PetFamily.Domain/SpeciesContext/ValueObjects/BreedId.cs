using PetFamily.Domain.PetsContext.ValueObjects.Pets;
using PetFamily.Domain.PetsContext.ValueObjects.Volunteers;
using PetFamily.Domain.Shared.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Domain.SpeciesContext.ValueObjects;
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
}
