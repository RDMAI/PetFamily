using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.DeletePetPhotos;

public record DeletePetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<string> PhotoPaths) : ICommand;
