using PetFamily.Application.PetsManagement.Volunteers.DTOs;

namespace PetFamily.Application.PetsManagement.Volunteers.UpdateRequisites;
public record UpdateRequisitesCommand(Guid VolunteerId, IEnumerable<RequisitesDTO> RequisitesList);
