using PetFamily.PetsManagement.Application.Pets.Commands.UpdatePet;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Contracts.Requests.Pets;

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
