using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.SpeciesManagement.Commands.DeleteSpecies;

public record DeleteSpeciesCommand(
    Guid SpeciesId) : ICommand;
