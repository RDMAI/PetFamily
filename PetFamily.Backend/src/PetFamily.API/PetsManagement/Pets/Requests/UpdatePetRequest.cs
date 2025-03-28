using PetFamily.Application.PetsManagement.Pets.Commands.AddPet;
using PetFamily.Application.PetsManagement.Pets.Commands.UpdatePet;
using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.PetsManagement.Pets.Requests;

public record UpdatePetRequest(
    UpdatePetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList)
{
    public UpdatePetCommand ToCommand(Guid volunteerId, Guid petId)
    {
        return new UpdatePetCommand(
            volunteerId,
            petId,
            Pet,
            Address,
            RequisitesList);
    }
}
