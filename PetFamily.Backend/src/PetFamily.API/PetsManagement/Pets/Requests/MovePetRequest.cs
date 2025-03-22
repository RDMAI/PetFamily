using PetFamily.Application.PetsManagement.Pets.Commands.MovePet;

namespace PetFamily.API.PetsManagement.Pets.Requests;

public record MovePetRequest(int NewSerialNumber)
{
    public MovePetCommand ToCommand(Guid volunteerId, Guid petId)
    {
        return new MovePetCommand(
            volunteerId,
            petId,
            NewSerialNumber);
    }
}