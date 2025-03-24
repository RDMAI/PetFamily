using CSharpFunctionalExtensions;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.Entities;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.SpeciesManagement.Interfaces;

public interface ISpeciesAggregateRepository
{
    Task<Result<Species, ErrorList>> GetByBreedIdAsync(
        BreedId breedId,
        CancellationToken cancellationToken = default);

    Task<Result<Species, ErrorList>> GetByIdAsync(
        SpeciesId Id,
        CancellationToken cancellationToken = default);
}
