namespace PetFamily.PetsManagement.Application.Pets.DTOs;

public class PetDTO
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public float Weight { get; init; }
    public float Height { get; init; }
    public Guid Breed_Id { get; init; }
    public Guid Species_Id { get; init; }
    public string Health_Information { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string Street { get; init; } = string.Empty;
    public string House_Number { get; init; } = string.Empty;
    public string? House_SubNumber { get; init; } = null;
    public string? Appartment_Number { get; init; } = null;
    public string Owner_Phone { get; init; } = string.Empty;
    public bool Is_Castrated { get; init; }
    public DateOnly Birth_Date { get; init; }
    public bool Is_Vacinated { get; init; }
    public int Status { get; init; }
    public int Serial_Number { get; init; }
    public Guid Volunteer_Id { get; init; }
    public bool Is_Deleted { get; init; }
}

