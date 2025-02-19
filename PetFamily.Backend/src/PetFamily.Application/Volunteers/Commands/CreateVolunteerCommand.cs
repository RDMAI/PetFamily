using PetFamily.Application.Volunteers.DTOs;

namespace PetFamily.Application.Volunteers.Commands;
public record CreateVolunteerCommand(
    VolunteerDTO VolunteerDTO,
    RequisitesDTO[] RequisitesList,
    SocialNetworkDTO[] SocialNetworksList)
{

}
