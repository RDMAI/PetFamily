using PetFamily.Shared.Core.DTOs;
using PetFamily.SpeciesManagement.Application.Queries.GetBreeds;

namespace PetFamily.SpeciesManagement.Contracts.Requests;

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
