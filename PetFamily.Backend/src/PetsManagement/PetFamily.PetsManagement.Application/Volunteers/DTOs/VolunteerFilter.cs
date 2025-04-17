using PetFamily.PetsManagement.Domain.ValueObjects.Volunteers;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.PetsManagement.Application.Volunteers.DTOs;
public record VolunteerFilter(VolunteerFullName? FullName = null, Phone? Phone = null, Email? Email = null);
