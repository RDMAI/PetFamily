using PetFamily.PetsManagement.Application.Pets.Commands.MovePet;

namespace PetFamily.PetsManagement.Contracts.Requests.Pets;

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