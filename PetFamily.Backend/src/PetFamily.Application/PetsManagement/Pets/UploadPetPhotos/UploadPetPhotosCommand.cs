using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;
public record UploadPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<FileDTO> Photos);
