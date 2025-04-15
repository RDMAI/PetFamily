using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetVolunteers;

public record GetVolunteersQuery(
    IEnumerable<SortByDTO> Sort,
    int CurrentPage,
    int PageSize) : IQuery;
