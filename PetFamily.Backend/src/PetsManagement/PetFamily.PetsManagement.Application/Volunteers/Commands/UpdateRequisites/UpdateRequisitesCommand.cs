using PetFamily.Application.Shared.DTOs;
using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateRequisites;
public record UpdateRequisitesCommand(
    Guid VolunteerId,
    IEnumerable<RequisitesDTO> RequisitesList) : ICommand;
