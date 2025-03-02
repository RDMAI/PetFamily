using PetFamily.API.PetsManagement.Volunteers.Requests;
using PetFamily.Application.PetsManagement.Volunteers.CreateVolunteer;

namespace PetFamily.API.Shared.Extensions;

public static class RequestExtensions
{
    public static CreateVolunteerCommand ToCommand(this CreateVolunteerRequest request)
    {
        return new CreateVolunteerCommand(
            request.FirstName,
            request.LastName,
            request.FatherName,
            request.Email,
            request.Description,
            request.ExperienceYears,
            request.Phone,
            request.RequisitesList,
            request.SocialNetworksList);
    }

    public static UpdateMainInfoCommand ToCommand(this UpdateMainInfoRequest request, Guid volunteerId)
    {
        return new UpdateMainInfoCommand(
            volunteerId,
            request.FirstName,
            request.LastName,
            request.FatherName,
            request.Email,
            request.Description,
            request.ExperienceYears,
            request.Phone);
    }
}
