using PetFamily.Application.Volunteers.DTOs;

namespace PetFamily.Application.Volunteers.Commands;
public record CreateVolunteerCommand(
    VolunteerDTO VolunteerDTO,
    IEnumerable<RequisitesDTO> RequisitesList,
    IEnumerable<SocialNetworkDTO> SocialNetworksList)
{

}
