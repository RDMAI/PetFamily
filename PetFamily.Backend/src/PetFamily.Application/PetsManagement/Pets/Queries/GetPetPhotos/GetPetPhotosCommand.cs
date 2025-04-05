namespace PetFamily.Application.PetsManagement.Pets.Queries.GetPetPhotos;

public record GetPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId);
