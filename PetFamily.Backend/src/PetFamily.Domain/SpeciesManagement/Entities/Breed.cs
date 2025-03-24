using CSharpFunctionalExtensions;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Domain.SpeciesManagement.Entities;

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
