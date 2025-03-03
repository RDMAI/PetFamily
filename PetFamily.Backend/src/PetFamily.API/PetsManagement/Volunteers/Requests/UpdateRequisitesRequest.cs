using PetFamily.Application.PetsManagement.Volunteers.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record UpdateRequisitesRequest(IEnumerable<RequisitesDTO> RequisitesList);
