namespace PetFamily.SpeciesManagement.Application.DTOs;

public class BreedDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid Species_Id { get; set; }
}
