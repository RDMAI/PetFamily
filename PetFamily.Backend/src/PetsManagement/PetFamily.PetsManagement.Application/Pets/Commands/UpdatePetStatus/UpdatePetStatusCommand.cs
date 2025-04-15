using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UpdatePetStatus;

public record UpdatePetStatusCommand(
    Guid VolunteerId,
    Guid PetId,
    int Status) : ICommand;
