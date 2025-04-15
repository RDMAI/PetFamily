using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Pets.Queries.GetPetById;

public record GetPetByIdQuery(Guid VolunteerId, Guid PetId) : IQuery;
