using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.SpeciesManagement.Commands.DeleteBreed;

public record DeleteBreedCommand(
    Guid SpeciesId,
    Guid BreedId) : ICommand;
