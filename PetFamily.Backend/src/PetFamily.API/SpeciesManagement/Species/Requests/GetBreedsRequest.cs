using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;

namespace PetFamily.API.SpeciesManagement.Species.Requests;

public record GetBreedsRequest(
    int CurrentPage,
    int PageSize,
    string? SortBy,
    bool IsSortAscending = true,
    string? Name = null)
{
    public GetBreedsQuery ToQuery(Guid SpeciesId)
    {
        List<SortByDTO> sorting = [];

        if (string.IsNullOrEmpty(SortBy) == false)
            sorting.Add(new(SortBy, IsSortAscending));

        return new GetBreedsQuery(sorting, CurrentPage, PageSize, SpeciesId, Name);
    }
}
