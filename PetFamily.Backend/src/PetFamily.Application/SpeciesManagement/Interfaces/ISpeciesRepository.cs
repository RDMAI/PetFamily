using CSharpFunctionalExtensions;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesContext.Entities;
using PetFamily.Domain.SpeciesContext.ValueObjects;

namespace PetFamily.Application.SpeciesManagement.Interfaces;

public interface ISpeciesRepository
{
    Task<Result<Species, ErrorList>> GetByBreedIdAsync(
        BreedId breedId,
        CancellationToken cancellationToken = default);

    Task<Result<Species, ErrorList>> GetByIdAsync(
        SpeciesId Id,
        CancellationToken cancellationToken = default);
}
