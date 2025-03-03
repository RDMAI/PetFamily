using PetFamily.Application.PetsManagement.Volunteers.DTOs;

namespace PetFamily.Application.PetsManagement.Volunteers.UpdateSocialNetworks;
public record UpdateSocialNetworksCommand(
    Guid VolunteerId,
    IEnumerable<SocialNetworkDTO> SocialNetworksList);
