using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.SpeciesManagement.Application.Commands.DeleteSpecies;

public record DeleteSpeciesCommand(
    Guid SpeciesId) : ICommand;
