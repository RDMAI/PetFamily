using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

public record UpdateRequisitesRequest(IEnumerable<RequisitesDTO> RequisitesList);
