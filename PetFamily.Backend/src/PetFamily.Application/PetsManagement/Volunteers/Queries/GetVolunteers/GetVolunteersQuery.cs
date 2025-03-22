using PetFamily.Application.Shared.Abstractions;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;

public record GetVolunteersQuery(
    int CurrentPage,
    int PageSize) : IQuery;
