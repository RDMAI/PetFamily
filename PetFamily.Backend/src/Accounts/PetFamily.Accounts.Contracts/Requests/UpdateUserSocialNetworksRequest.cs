using PetFamily.Accounts.Application.Commands.UpdateUserSocialNetworks;
using PetFamily.Accounts.Application.DTOs;

namespace PetFamily.Accounts.Contracts.Requests;

public record UpdateUserSocialNetworksRequest(
    List<SocialNetworkDTO> SocialNetworks)
{
    public UpdateUserSocialNetworksCommand ToCommand(Guid UserId)
    {
        return new UpdateUserSocialNetworksCommand(UserId, SocialNetworks);
    }
}
