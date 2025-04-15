using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.MovePet;

public record MovePetCommand(
    Guid VolunteerId,
    Guid PetId,
    int NewSerialNumber) : ICommand;
