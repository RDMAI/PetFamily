using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;

namespace PetFamily.SpeciesManagement.Application.Queries.GetSpecies;

public record GetSpeciesQuery(
    IEnumerable<SortByDTO> Sort,
    int CurrentPage,
    int PageSize,
    string? Name = null) : IQuery;
