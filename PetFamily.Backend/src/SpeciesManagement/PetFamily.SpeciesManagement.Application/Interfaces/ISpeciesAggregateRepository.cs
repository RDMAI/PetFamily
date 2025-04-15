using CSharpFunctionalExtensions;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.Entities;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.SpeciesManagement.Application.Interfaces;

public interface ISpeciesAggregateRepository
{
    Task<Result<Species, ErrorList>> GetByBreedIdAsync(
        BreedId breedId,
        CancellationToken cancellationToken = default);

    Task<Result<Species, ErrorList>> GetByIdAsync(
        SpeciesId Id,
        CancellationToken cancellationToken = default);

    Task<Result<SpeciesId, ErrorList>> HardDeleteAsync(
        Species entity,
        CancellationToken cancellationToken = default);

    Task<Result<SpeciesId, ErrorList>> UpdateAsync(
        Species entity,
        CancellationToken cancellationToken = default);
}
