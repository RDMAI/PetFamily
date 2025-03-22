using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.DTOs;

public class VolunteerDTO
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string FatherName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public float ExperienceYears { get; init; }
    public string Phone { get; init; } = string.Empty;
    public Requisites[] Requisites { get; init; } = [];
    public SocialNetwork[] SocialNetworks { get; init; } = [];
    public ICollection<PetDTO> Pets { get; init; } = [];
    public bool IsDeleted {  get; init; }
}
