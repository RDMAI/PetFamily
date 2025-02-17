using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;
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

    public static Result<Species, Error> Create(SpeciesId id, string name, List<Breed> breeds)
    {
        if (string.IsNullOrWhiteSpace(name))
            return ErrorHelper.General.ValueIsNullOrEmpty("Name");

        return new Species(id, name, breeds);
    }

    public Result<Species, Error> AddBreed(Breed breed)
    {
        if (breed is null)
            return ErrorHelper.General.ValueIsNullOrEmpty("Breed");

        _breeds.Add(breed);
        return this;
    }
}
