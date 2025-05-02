using PetFamily.Accounts.Application.DTOs;
using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.Accounts.Application.Commands.UpdateUserSocialNetworks;

public record UpdateUserSocialNetworksCommand(
    Guid UserId,
    List<SocialNetworkDTO> SocialNetworks) : ICommand;
