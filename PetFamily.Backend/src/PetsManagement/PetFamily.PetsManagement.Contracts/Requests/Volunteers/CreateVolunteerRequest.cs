using PetFamily.PetsManagement.Application.Volunteers.Commands.CreateVolunteer;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Contracts.Requests.Volunteers;

public record CreateVolunteerRequest(
    string FirstName,
    string LastName,
    string FatherName,
    string Email,
    string Description,
    float ExperienceYears,
    string Phone,
    IEnumerable<RequisitesDTO> RequisitesList,
    IEnumerable<SocialNetworkDTO> SocialNetworksList)
{
    public CreateVolunteerCommand ToCommand()
    {
        return new CreateVolunteerCommand(
            FirstName,
            LastName,
            FatherName,
            Email,
            Description,
            ExperienceYears,
            Phone,
            RequisitesList,
            SocialNetworksList);
    }
}
