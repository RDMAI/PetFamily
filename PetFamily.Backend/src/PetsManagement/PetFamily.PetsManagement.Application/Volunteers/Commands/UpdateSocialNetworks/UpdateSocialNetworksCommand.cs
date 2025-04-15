using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateSocialNetworks;
public record UpdateSocialNetworksCommand(
    Guid VolunteerId,
    IEnumerable<SocialNetworkDTO> SocialNetworksList) : ICommand;
