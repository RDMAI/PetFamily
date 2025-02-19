using PetFamily.Application.Volunteers.DTOs;

namespace PetFamily.API.Volunteers.Requests;

public record CreateVolunteerRequest(
    VolunteerDTO VolunteerDTO,
    RequisitesDTO[] RequisitesList,
    SocialNetworkDTO[] SocialNetworksList)
{

}
