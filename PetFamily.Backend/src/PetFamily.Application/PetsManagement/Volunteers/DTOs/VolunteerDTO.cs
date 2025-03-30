using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.DTOs;

public class VolunteerDTO
{
    public Guid Id { get; init; }
    public string First_Name { get; init; } = string.Empty;
    public string Last_Name { get; init; } = string.Empty;
    public string Father_Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public float Experience_Years { get; init; }
    public string Phone { get; init; } = string.Empty;
    public Requisites[] Requisites { get; init; } = [];
    public SocialNetwork[] Social_Networks { get; init; } = [];
    public ICollection<PetDTO> Pets { get; init; } = [];
    public bool Is_Deleted {  get; init; }
}
