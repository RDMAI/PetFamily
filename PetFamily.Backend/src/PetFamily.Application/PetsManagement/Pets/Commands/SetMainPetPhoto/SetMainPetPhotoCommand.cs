using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Pets.Commands.SetMainPetPhoto;

public record SetMainPetPhotoCommand(
    Guid VolunteerId,
    Guid PetId,
    string PhotoPath) : ICommand;
