using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;

namespace PetFamily.Application.PetsManagement.Pets.Queries.GetPets;

public record GetPetsQuery(
    IEnumerable<SortByDTO> Sort,
    int CurrentPage,
    int PageSize,
    Guid? SpeciesId,
    Guid? BreedId) : IQuery;
