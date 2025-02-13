using CSharpFunctionalExtensions;
using PetFamily.Domain.PetsContext.Entities;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetBreed
{
    public Guid BreedId { get; }
    public Guid SpeciesId { get; }

    public static Result<PetBreed> Create(BreedId breedId, SpeciesId speciesId)
    {
        if (breedId is null) return Result.Failure<PetBreed>("BreedId cannot be null");
        if (speciesId is null) return Result.Failure<PetBreed>("SpeciesId cannot be null");

        return Result.Success(new PetBreed(breedId, speciesId));
    }

    private PetBreed(BreedId breedId, SpeciesId speciesId)
    {
        BreedId = breedId.Value;
        SpeciesId = speciesId.Value;
    }
}
