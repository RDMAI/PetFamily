using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Breed : Entity<BreedId>
{
    // EF Core
    private Breed() { }

    public Breed(BreedId id, BreedName name)
    {
        Id = id;
        Name = name;
    }

    public BreedName Name { get; private set; }
}
