using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Pets.Commands.MovePet;

public record MovePetCommand(
    Guid VolunteerId,
    Guid PetId,
    int NewSerialNumber) : ICommand;
