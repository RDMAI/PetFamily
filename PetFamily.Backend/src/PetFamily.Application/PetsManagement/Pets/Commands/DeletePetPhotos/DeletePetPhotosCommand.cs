using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Pets.Commands.DeletePetPhotos;

public record DeletePetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<string> PhotoPaths) : ICommand;
