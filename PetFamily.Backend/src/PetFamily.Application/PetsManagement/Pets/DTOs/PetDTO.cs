namespace PetFamily.Application.PetsManagement.Pets.DTOs;

public class PetDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public float Weight { get; init; }
    public float Height { get; init; }
    public Guid BreedId { get; init; }
    public Guid SpeciesId { get; init; }
    public string HealthInformation { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string HouseNumber { get; init; } = string.Empty;
    public string? HouseSubNumber { get; init; } = null;
    public string? AppartmentNumber { get; init; } = null;
    public string OwnerPhone { get; init; } = string.Empty;
    public bool IsCastrated { get; init; }
    public DateOnly BirthDate { get; init; }
    public bool IsVacinated { get; init; }
    public int Status { get; init; }
    public int SerialNumber { get; init; }
    public Guid VolunteerId { get; init; }
    public bool IsDeleted { get; init; }
}
    
