using PetFamily.PetsManagement.Application.Volunteers.Queries.GetVolunteers;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.PetsManagement.Contracts.Requests.Volunteers;

public record GetVolunteersRequest(
    int CurrentPage,
    int PageSize,
    string? SortBy,
    bool IsSortAscending = true)
{
    public GetVolunteersQuery ToQuery()
    {
        List<SortByDTO> sorting = [];

        if (string.IsNullOrEmpty(SortBy) == false)
            sorting.Add(new(SortBy, IsSortAscending));

        return new GetVolunteersQuery(sorting, CurrentPage, PageSize);
    }
}
