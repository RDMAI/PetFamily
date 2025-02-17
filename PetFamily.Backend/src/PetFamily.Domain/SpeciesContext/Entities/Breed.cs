using CSharpFunctionalExtensions;
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

    public static Result<Breed> Create(BreedId id, string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<Breed>("Breed name cannot be empty");

        return Result.Success(new Breed(id, name));
    }
}
