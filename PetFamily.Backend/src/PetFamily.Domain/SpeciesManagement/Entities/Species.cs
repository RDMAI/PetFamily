using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Domain.SpeciesManagement.Entities;

public class Species : Entity<SpeciesId>
{
    // EF Core
    private Species() { }
    public Species(SpeciesId id, SpeciesName name, List<Breed> breeds)
    {
        Id = id;
        Name = name;
        _breeds = breeds;
    }
    public SpeciesName Name { get; private set; }
    public IReadOnlyList<Breed> Breeds => _breeds;
    private List<Breed> _breeds = [];

    public Result<Species, Error> AddBreed(Breed breed)
    {
        if (breed is null)
            return ErrorHelper.General.ValueIsNullOrEmpty("Breed");

        _breeds.Add(breed);
        return this;
    }

    public Result<Species, Error> DeleteBreed(BreedId breedId)
    {
        _breeds.RemoveAll(b => b.Id == breedId);

        return this;
    }
}
