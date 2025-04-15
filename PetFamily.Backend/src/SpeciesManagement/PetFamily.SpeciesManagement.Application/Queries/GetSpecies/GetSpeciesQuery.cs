using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.SpeciesManagement.Application.Queries.GetSpecies;

public record GetSpeciesQuery(
    IEnumerable<SortByDTO> Sort,
    int CurrentPage,
    int PageSize,
    string? Name = null) : IQuery;
