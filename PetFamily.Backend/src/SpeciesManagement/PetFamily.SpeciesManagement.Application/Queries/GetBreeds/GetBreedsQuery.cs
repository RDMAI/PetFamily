using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.SpeciesManagement.Application.Queries.GetBreeds;

public record GetBreedsQuery(
    IEnumerable<SortByDTO> Sort,
    int CurrentPage,
    int PageSize,
    Guid SpeciesId,
    string? Name = null) : IQuery;
