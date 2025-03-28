using PetFamily.Application.PetsManagement.Pets.Commands.SetMainPetPhoto;

namespace PetFamily.API.PetsManagement.Pets.Requests;

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
