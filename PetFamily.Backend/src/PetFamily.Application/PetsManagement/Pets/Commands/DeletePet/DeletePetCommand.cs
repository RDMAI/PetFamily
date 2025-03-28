using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Pets.Commands.DeletePet;

public record DeletePetCommand(
    Guid VolunteerId,
    Guid PetId) : ICommand;
