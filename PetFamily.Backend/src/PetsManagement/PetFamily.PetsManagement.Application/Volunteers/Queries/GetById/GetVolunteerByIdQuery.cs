using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetById;

public record GetVolunteerByIdQuery(Guid Id) : IQuery;
