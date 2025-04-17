using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Domain.ValueObjects;

namespace PetFamily.SpeciesManagement.Domain.Entities;

public class Species : Entity<SpeciesId>
{
    // EF Core
    private Species() { }
    public Species(SpeciesId id, SpeciesName name)
    {
        Id = id;
        Name = name;
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
