using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Volunteers.DTOs;
public record VolunteerFilter(VolunteerFullName? FullName = null, Phone? Phone = null, Email? Email = null);
