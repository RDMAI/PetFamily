using PetFamily.API.PetsManagement.Volunteers.Requests;
using PetFamily.Application.PetsManagement.Pets.AddPet;
using PetFamily.Application.PetsManagement.Pets.MovePet;
using PetFamily.Application.PetsManagement.Volunteers.CreateVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.UpdateMainInfo;
using PetFamily.Application.PetsManagement.Volunteers.UpdateRequisites;
using PetFamily.Application.PetsManagement.Volunteers.UpdateSocialNetworks;

namespace PetFamily.API.PetsManagement.Volunteers.Extensions;

public static class VolunteerRequestExtensions
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

    public static UpdateSocialNetworksCommand ToCommand(this UpdateSocialNetworksRequest request, Guid volunteerId)
    {
        return new UpdateSocialNetworksCommand(
            volunteerId,
            request.SocialNetworksList);
    }

    public static UpdateRequisitesCommand ToCommand(this UpdateRequisitesRequest request, Guid volunteerId)
    {
        return new UpdateRequisitesCommand(
            volunteerId,
            request.RequisitesList);
    }

    public static AddPetCommand ToCommand(this AddPetRequest request, Guid volunteerId)
    {
        return new AddPetCommand(
            volunteerId,
            request.Pet,
            request.Address,
            request.RequisitesList);
    }

    public static MovePetCommand ToCommand(this MovePetRequest request, Guid volunteerId, Guid petId)
    {
        return new MovePetCommand(
            volunteerId,
            petId,
            request.NewSerialNumber);
    }
}
