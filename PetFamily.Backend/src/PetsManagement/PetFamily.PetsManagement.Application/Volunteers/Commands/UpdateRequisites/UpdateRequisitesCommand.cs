using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateRequisites;
public record UpdateRequisitesCommand(
    Guid VolunteerId,
    IEnumerable<RequisitesDTO> RequisitesList) : ICommand;
