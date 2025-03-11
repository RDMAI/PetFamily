using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Volunteers.UpdateRequisites;
public record UpdateRequisitesCommand(Guid VolunteerId, IEnumerable<RequisitesDTO> RequisitesList);
