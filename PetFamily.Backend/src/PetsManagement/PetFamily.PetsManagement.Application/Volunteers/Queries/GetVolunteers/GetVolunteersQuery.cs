using PetFamily.Application.Shared.DTOs;
using PetFamily.Shared.Core.Application.Abstractions;

namespace PetFamily.PetsManagement.Application.Volunteers.Queries.GetVolunteers;

public record GetVolunteersQuery(
    IEnumerable<SortByDTO> Sort,
    int CurrentPage,
    int PageSize) : IQuery;
