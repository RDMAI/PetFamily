using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Breed : Entity<BreedId>
{
    public string Name { get; private set; }

    // EF Core
    private Breed() { }

    private Breed(BreedId id, string name)
    {
        Id = id;
        Name = name;
    }

    public static Result<Breed, Error> Create(BreedId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name) || name.Length > Constants.MAX_LOW_TEXT_LENGTH)
            return ErrorHelper.General.ValueIsInvalid("Name");

        return new Breed(id, name);
    }
}
