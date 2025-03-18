namespace PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;

public record DeletePetPhotosCommand(
    Guid VolunteerId,
    Guid PetId,
    IEnumerable<string> PhotoPaths);
