using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Pets.Queries.GetPetById;

public record GetPetByIdQuery(Guid VolunteerId, Guid PetId) : IQuery;
