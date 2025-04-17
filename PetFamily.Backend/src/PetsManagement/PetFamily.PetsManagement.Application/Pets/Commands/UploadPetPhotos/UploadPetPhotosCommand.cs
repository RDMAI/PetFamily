using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Files;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UploadPetPhotos;

public record UploadPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<FileDTO> Photos) : ICommand;
