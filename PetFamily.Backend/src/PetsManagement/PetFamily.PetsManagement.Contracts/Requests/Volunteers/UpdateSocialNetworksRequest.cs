using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;

namespace PetFamily.PetsManagement.Contracts.Requests.Volunteers;

public record UpdateSocialNetworksRequest(IEnumerable<SocialNetworkDTO> SocialNetworksList)
{
    public UpdateSocialNetworksCommand ToCommand(Guid volunteerId)
    {
        return new UpdateSocialNetworksCommand(
            volunteerId,
            SocialNetworksList);
    }
}
