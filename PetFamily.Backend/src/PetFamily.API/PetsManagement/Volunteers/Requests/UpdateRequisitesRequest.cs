using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateRequisites;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record UpdateRequisitesRequest(IEnumerable<RequisitesDTO> RequisitesList)
{
    public UpdateRequisitesCommand ToCommand(Guid volunteerId)
    {
        return new UpdateRequisitesCommand(
            volunteerId,
            RequisitesList);
    }
}
