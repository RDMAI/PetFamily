using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;

public record GetVolunteerByIdQuery(Guid Id) : IQuery;
