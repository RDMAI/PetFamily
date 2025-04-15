using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.SpeciesManagement.Application.Commands.DeleteBreed;

public record DeleteBreedCommand(
    Guid SpeciesId,
    Guid BreedId) : ICommand;
