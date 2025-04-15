using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.DeletePet;

public record DeletePetCommand(
    Guid VolunteerId,
    Guid PetId) : ICommand;
