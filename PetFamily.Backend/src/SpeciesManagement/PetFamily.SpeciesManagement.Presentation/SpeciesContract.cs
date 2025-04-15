using CSharpFunctionalExtensions;
using PetFamily.Shared.Kernel;
using PetFamily.SpeciesManagement.Application.DTOs;
using PetFamily.SpeciesManagement.Contracts;
using System.Data;

namespace PetFamily.SpeciesManagement.Presentation;

public class SpeciesContract : ISpeciesContract
{
    public Task<Result<BreedDTO, ErrorList>> GetBreedByIdAsync(
        IDbConnection connection,
        Guid BreedId,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
