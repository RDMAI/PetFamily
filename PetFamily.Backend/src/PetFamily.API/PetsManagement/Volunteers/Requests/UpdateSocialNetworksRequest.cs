using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record UpdateSocialNetworksRequest(IEnumerable<SocialNetworkDTO> SocialNetworksList)
{
    public UpdateSocialNetworksCommand ToCommand(Guid volunteerId)
    {
        return new UpdateSocialNetworksCommand(
            volunteerId,
            SocialNetworksList);
    }
}
