using PetFamily.API.PetsManagement.Pets.Requests;
using PetFamily.Application.PetsManagement.Pets.AddPet;
using PetFamily.Application.PetsManagement.Pets.MovePet;

namespace PetFamily.API.PetsManagement.Pets.Extensions;

public static class PetRequestExtensions
{
    public static AddPetCommand ToCommand(
        this AddPetRequest request,
        Guid volunteerId)
    {
        return new AddPetCommand(
            volunteerId,
            request.Pet,
            request.Address,
            request.RequisitesList);
    }

    public static MovePetCommand ToCommand(
        this MovePetRequest request,
        Guid volunteerId,
        Guid petId)
    {
        return new MovePetCommand(
            volunteerId,
            petId,
            request.NewSerialNumber);
    }
}
