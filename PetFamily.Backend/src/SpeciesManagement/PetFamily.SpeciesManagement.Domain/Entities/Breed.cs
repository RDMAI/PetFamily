using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Domain.ValueObjects;

namespace PetFamily.SpeciesManagement.Domain.Entities;

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
