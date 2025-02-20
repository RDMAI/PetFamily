using PetFamily.API.Volunteers.Requests;
using PetFamily.Application.Volunteers.Commands;
using PetFamily.Application.Volunteers.DTOs;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.API.Extensions;

public static class RequestExtensions
{
    public static CreateVolunteerCommand ToCommand(this CreateVolunteerRequest request)
    {
        return new CreateVolunteerCommand(
            request.VolunteerDTO,
            request.RequisitesList,
            request.SocialNetworksList);
    }
}
