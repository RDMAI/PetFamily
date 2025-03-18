namespace PetFamily.Application.PetsManagement.Pets.MovePet;

public record MovePetCommand(
    Guid VolunteerId,
    Guid PetId,
    int NewSerialNumber);
