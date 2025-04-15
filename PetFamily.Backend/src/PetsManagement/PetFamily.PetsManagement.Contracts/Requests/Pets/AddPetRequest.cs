using PetFamily.PetsManagement.Application.Pets.Commands.AddPet;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Contracts.Requests.Pets;

public record AddPetRequest(
    AddPetDTO Pet,
    AddressDTO Address,
    IEnumerable<RequisitesDTO> RequisitesList)
{
    public AddPetCommand ToCommand(Guid volunteerId)
    {
        return new AddPetCommand(
            volunteerId,
            Pet,
            Address,
            RequisitesList);
    }
}
