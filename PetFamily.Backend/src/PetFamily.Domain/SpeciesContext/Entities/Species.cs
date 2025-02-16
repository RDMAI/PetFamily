using CSharpFunctionalExtensions;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Domain.SpeciesContext.Entities;

public class Species : Entity<SpeciesId>
{
    public string Name { get; private set; }
    public IReadOnlyList<Breed> Breeds => _breeds;
    private List<Breed> _breeds = [];

    // EF Core
    private Species() { }

    private Species(SpeciesId id, string name, List<Breed> breeds)
    {
        Id = id;
        Name = name;
        _breeds = breeds;
    }

    public static Result<Species> Create(SpeciesId id, string name, List<Breed> breeds)
    {
        if (string.IsNullOrWhiteSpace(name)) return Result.Failure<Species>("Species name cannot be empty");

        return Result.Success(new Species(id, name, breeds));
    }

    public Result<Species> AddBreed(Breed breed)
    {
        if (breed is null) return Result.Failure<Species>("Breed cannot be empty");

        _breeds.Add(breed);
        return Result.Success(this);
    }
}
