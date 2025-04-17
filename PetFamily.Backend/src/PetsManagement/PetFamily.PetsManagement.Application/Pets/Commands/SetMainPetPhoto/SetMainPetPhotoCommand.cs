using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.SetMainPetPhoto;

public record SetMainPetPhotoCommand(
    Guid VolunteerId,
    Guid PetId,
    string PhotoPath) : ICommand;
