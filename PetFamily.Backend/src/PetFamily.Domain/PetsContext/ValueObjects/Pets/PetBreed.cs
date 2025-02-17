using CSharpFunctionalExtensions;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetBreed
{
    public Guid BreedId { get; }
    public Guid SpeciesId { get; }

    public static Result<PetBreed> Create(Guid breedId, Guid speciesId) =>
        Result.Success(new PetBreed(breedId, speciesId));

    private PetBreed(Guid breedId, Guid speciesId)
    {
        BreedId = breedId;
        SpeciesId = speciesId;
    }
}
