using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record AddPetRequest(
    PetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList);
