using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.Queries.GetSpecies;

namespace PetFamily.API.SpeciesManagement.Species.Requests;

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
