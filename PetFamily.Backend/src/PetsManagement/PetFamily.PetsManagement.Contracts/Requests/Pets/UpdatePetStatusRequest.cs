using PetFamily.PetsManagement.Application.Pets.Commands.UpdatePetStatus;

namespace PetFamily.PetsManagement.Contracts.Requests.Pets;

public record UpdatePetStatusRequest(int status)
{
    public UpdatePetStatusCommand ToCommand(Guid volunteerId, Guid petId)
    {
        return new UpdatePetStatusCommand(
            volunteerId,
            petId,
            status);
    }
}
