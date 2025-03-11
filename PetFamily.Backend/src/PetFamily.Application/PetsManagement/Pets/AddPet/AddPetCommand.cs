using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Pets.AddPet;

public record AddPetCommand(
    Guid VolunteerId,
    PetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList);
