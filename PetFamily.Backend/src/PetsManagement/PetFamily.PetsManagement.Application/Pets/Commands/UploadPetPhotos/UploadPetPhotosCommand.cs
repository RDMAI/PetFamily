using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UploadPetPhotos;

public record UploadPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<Shared.Core.Files.FileDTO> Photos) : ICommand;
