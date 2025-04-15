using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UpdatePetStatus;

public record UpdatePetStatusCommand(
    Guid VolunteerId,
    Guid PetId,
    int Status) : ICommand;
