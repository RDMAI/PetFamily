using PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.API.PetsManagement.Volunteers.Requests;

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
