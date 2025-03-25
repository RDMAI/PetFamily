using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;

public record GetVolunteersQuery(
    IEnumerable<SortByDTO> Sort,
    int CurrentPage,
    int PageSize) : IQuery;
