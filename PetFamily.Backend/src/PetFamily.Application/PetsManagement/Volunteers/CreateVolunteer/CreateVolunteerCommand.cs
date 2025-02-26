using PetFamily.Application.PetsManagement.Volunteers.DTOs;

namespace PetFamily.Application.PetsManagement.Volunteers.CreateVolunteer;
public record CreateVolunteerCommand(
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone,
    IEnumerable<RequisitesDTO> RequisitesList,
    IEnumerable<SocialNetworkDTO> SocialNetworksList);
