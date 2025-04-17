using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateRequisites;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Contracts.Requests.Volunteers;

public record UpdateRequisitesRequest(IEnumerable<RequisitesDTO> RequisitesList)
{
    public UpdateRequisitesCommand ToCommand(Guid volunteerId)
    {
        return new UpdateRequisitesCommand(
            volunteerId,
            RequisitesList);
    }
}
