namespace PetFamily.Application.SpeciesManagement.DTOs;

public class BreedDTO
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid SpeciesId { get; set; }
}
