using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateRequisites;
public record UpdateRequisitesCommand(
    Guid VolunteerId,
    IEnumerable<RequisitesDTO> RequisitesList) : ICommand;
