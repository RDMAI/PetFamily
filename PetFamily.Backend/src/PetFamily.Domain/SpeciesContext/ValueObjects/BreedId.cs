using PetFamily.Domain.PetsContext.ValueObjects.Pets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetFamily.Domain.SpeciesContext.ValueObjects;
public record BreedId : IComparable<BreedId>
{
    public Guid Value { get; }

    //IComparable<PetId>
    public int CompareTo(BreedId? other) => Value.CompareTo(other?.Value);
}
