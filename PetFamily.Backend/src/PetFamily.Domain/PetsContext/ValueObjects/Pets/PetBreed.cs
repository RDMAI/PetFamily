using CSharpFunctionalExtensions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetBreed
{
    public Guid BreedId { get; }
    public Guid SpeciesId { get; }

    public static Result<PetBreed, Error> Create(Guid breedId, Guid speciesId)
    {
        if (breedId == Guid.Empty)
            return ErrorHelper.General.ValueIsInvalid("Breed Id");
        if (speciesId == Guid.Empty)
            return ErrorHelper.General.ValueIsInvalid("Species Id");

        return new PetBreed(breedId, speciesId);
    }

    private PetBreed(Guid breedId, Guid speciesId)
    {
        BreedId = breedId;
        SpeciesId = speciesId;
    }
}
