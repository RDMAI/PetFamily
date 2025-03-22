using PetFamily.API.Shared.Requests;
using PetFamily.Application.PetsManagement.Pets.Commands.DeletePetPhotos;

namespace PetFamily.API.PetsManagement.Pets.Extensions;

public static class PetRequestExtensions
{
    public static DeletePetPhotosCommand ToCommand(
        this DeleteFilesRequest request,
        Guid volunteerId,
        Guid petId)
    {
        return new DeletePetPhotosCommand(
            volunteerId,
            petId,
            request.FilePaths);
    }

    //public static GetPetPhotosCommand ToCommand(
    //    this GetFilesRequest request,
    //    Guid volunteerId,
    //    Guid petId)
    //{
    //    return new GetPetPhotosCommand(
    //        volunteerId,
    //        petId,
    //        request.FilePaths);
    //}
}
