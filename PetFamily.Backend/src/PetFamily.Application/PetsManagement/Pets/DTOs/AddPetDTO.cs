namespace PetFamily.Application.PetsManagement.Pets.DTOs;

public record AddPetDTO(
    string Name,
    string Description,
    string Color,
    float Weight,
    float Height,
    Guid BreedId,
    string HealthInformation,
    string OwnerPhone,
    bool IsCastrated,
    DateOnly BirthDate,
    bool IsVacinated,
    int Status);
