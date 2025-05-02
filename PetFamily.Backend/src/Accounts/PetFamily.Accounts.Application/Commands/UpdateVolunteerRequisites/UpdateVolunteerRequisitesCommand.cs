using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.Accounts.Application.Commands.UpdateVolunteerRequisites;

public record UpdateVolunteerRequisitesCommand(
    Guid UserId,
    List<RequisitesDTO> Requisites) : ICommand;
