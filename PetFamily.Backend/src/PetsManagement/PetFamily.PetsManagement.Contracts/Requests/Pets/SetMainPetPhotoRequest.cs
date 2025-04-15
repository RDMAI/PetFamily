using PetFamily.PetsManagement.Application.Pets.Commands.SetMainPetPhoto;

namespace PetFamily.PetsManagement.Contracts.Requests.Pets;

public record SetMainPetPhotoRequest(string PhotoPath)
{
    public SetMainPetPhotoCommand ToCommand(Guid volunteerId, Guid petId)
    {
        return new SetMainPetPhotoCommand(
            volunteerId,
            petId,
            PhotoPath);
    }
}
