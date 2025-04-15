namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPetPhotos;

public record GetPetPhotosCommand(
    Guid VolunteerId,
    Guid PetId);
