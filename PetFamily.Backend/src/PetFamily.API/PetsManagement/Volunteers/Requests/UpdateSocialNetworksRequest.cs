using PetFamily.Application.PetsManagement.Volunteers.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record UpdateSocialNetworksRequest(IEnumerable<SocialNetworkDTO> SocialNetworksList);
