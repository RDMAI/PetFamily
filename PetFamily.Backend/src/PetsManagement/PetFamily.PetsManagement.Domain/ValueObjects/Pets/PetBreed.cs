using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.PetsManagement.Domain.ValueObjects.Pets;

public record PetBreed
{
    public BreedId BreedId { get; }
    public SpeciesId SpeciesId { get; }

    public static Result<PetBreed, Error> Create(BreedId breedId, SpeciesId speciesId)
    {
        if (breedId == BreedId.Empty())
            return ErrorHelper.General.ValueIsInvalid("Breed Id");
        if (speciesId == SpeciesId.Empty())
            return ErrorHelper.General.ValueIsInvalid("Species Id");

        return new PetBreed(breedId, speciesId);
    }

    private PetBreed(BreedId breedId, SpeciesId speciesId)
    {
        BreedId = breedId;
        SpeciesId = speciesId;
    }
}
