using PetFamily.Application.PetsManagement.Pets.Commands.UpdatePetStatus;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;

namespace PetFamily.API.PetsManagement.Pets.Requests;

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
