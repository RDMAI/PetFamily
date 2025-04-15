using PetFamily.PetsManagement.Application.Pets.Commands.DeletePetPhotos;

namespace PetFamily.PetsManagement.Contracts.Requests.Pets;

public record DeletePetPhotosRequest(IEnumerable<string> FilePaths)
{
    public DeletePetPhotosCommand ToCommand(
        Guid volunteerId,
        Guid petId)
    {
        return new DeletePetPhotosCommand(
            volunteerId,
            petId,
            FilePaths);
    }
}
