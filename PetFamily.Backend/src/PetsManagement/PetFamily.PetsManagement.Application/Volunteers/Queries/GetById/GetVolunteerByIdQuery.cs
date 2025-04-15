using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetById;

public record GetVolunteerByIdQuery(Guid Id) : IQuery;
