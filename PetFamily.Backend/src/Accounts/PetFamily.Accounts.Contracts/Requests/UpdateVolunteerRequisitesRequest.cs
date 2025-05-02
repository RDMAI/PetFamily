using PetFamily.Accounts.Application.Commands.UpdateVolunteerRequisites;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.Accounts.Contracts.Requests;

public record UpdateVolunteerRequisitesRequest(
    List<RequisitesDTO> Requisites)
{
    public UpdateVolunteerRequisitesCommand ToCommand(Guid userId)
    {
        return new UpdateVolunteerRequisitesCommand(userId, Requisites);
    }
}
