using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;

namespace PetFamily.Domain.PetsContext.ValueObjects.Pets;

public record PetBreed
{
    public Guid BreedId { get; }
    public Guid SpeciesId { get; }

    public static Result<PetBreed, Error> Create(Guid breedId, Guid speciesId) =>
        new PetBreed(breedId, speciesId);

    private PetBreed(Guid breedId, Guid speciesId)
    {
        BreedId = breedId;
        SpeciesId = speciesId;
    }
}
