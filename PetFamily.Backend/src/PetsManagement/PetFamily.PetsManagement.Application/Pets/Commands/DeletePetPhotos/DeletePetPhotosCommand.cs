using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.DeletePetPhotos;

public record DeletePetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<string> PhotoPaths) : ICommand;
