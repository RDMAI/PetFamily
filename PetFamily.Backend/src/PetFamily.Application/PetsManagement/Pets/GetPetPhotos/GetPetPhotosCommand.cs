namespace PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;

public record GetPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId);
