using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;
using PetFamily.SpeciesManagement.Application.DTOs;
using System.Data;

namespace PetFamily.SpeciesManagement.Contracts;

public interface ISpeciesContract
{
    public Task<Result<BreedDTO, ErrorList>> GetBreedByIdAsync(
        IDbConnection connection,
        Guid BreedId,
        CancellationToken cancellationToken = default);
}
