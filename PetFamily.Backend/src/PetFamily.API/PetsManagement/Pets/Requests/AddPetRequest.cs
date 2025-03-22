using PetFamily.Application.PetsManagement.Pets.Commands.AddPet;
using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.PetsManagement.Pets.Requests;

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
