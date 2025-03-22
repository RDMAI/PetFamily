using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateSocialNetworks;
public record UpdateSocialNetworksCommand(
    Guid VolunteerId,
    IEnumerable<SocialNetworkDTO> SocialNetworksList) : ICommand;
