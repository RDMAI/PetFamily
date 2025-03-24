using CSharpFunctionalExtensions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;
using PetFamily.Application.SpeciesManagement.Queries.GetSpecies;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.SpeciesManagement.Interfaces;

public interface ISpeciesAggregateDBReader
{
    public Task<Result<DataListPage<SpeciesDTO>, ErrorList>> GetAsync(
        GetSpeciesQuery query,
        CancellationToken cancellationToken = default);

    public Task<Result<SpeciesDTO, ErrorList>> GetByIdAsync(
        Guid SpeciesId,
        CancellationToken cancellationToken = default);

    public Task<Result<DataListPage<BreedDTO>, ErrorList>> GetBreedsAsync(
        GetBreedsQuery query,
        CancellationToken cancellationToken = default);

    public Task<Result<BreedDTO, ErrorList>> GetBreedByIdAsync(
        Guid BreedId,
        CancellationToken cancellationToken = default);
}
