using PetFamily.Application.PetsManagement.Volunteers.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record CreateVolunteerRequest(
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone,
    IEnumerable<RequisitesDTO> RequisitesList,
    IEnumerable<SocialNetworkDTO> SocialNetworksList);
