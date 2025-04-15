using PetFamily.Shared.Core.DTOs;
using PetFamily.SpeciesManagement.Application.Queries.GetSpecies;

namespace PetFamily.SpeciesManagement.Contracts.Requests;

public record GetSpeciesRequest(
    string? Name,
    int CurrentPage,
    int PageSize,
    string? SortBy,
    bool IsSortAscending = true)
{
    public GetSpeciesQuery ToQuery()
    {
        List<SortByDTO> sorting = [];

        if (string.IsNullOrEmpty(SortBy) == false)
            sorting.Add(new(SortBy, IsSortAscending));

        return new GetSpeciesQuery(sorting, CurrentPage, PageSize, Name);
    }
}
