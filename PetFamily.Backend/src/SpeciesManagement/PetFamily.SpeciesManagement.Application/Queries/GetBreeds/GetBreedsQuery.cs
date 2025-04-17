using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.SpeciesManagement.Application.Queries.GetBreeds;

public record GetBreedsQuery(
    IEnumerable<SortByDTO> Sort,
    int CurrentPage,
    int PageSize,
    Guid SpeciesId,
    string? Name = null) : IQuery;
