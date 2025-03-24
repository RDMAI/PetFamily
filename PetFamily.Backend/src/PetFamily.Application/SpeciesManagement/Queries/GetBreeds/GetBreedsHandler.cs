using CSharpFunctionalExtensions;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.SpeciesManagement.Queries.GetSpecies;

public class GetBreedsHandler
    : IQueryHandler<DataListPage<BreedDTO>, GetBreedsQuery>
{
    private readonly ISpeciesAggregateDBReader _dbReader;
    private readonly GetBreedsQueryValidator _validator;

    public GetBreedsHandler(
        ISpeciesAggregateDBReader dbReader,
        GetBreedsQueryValidator validator)
    {
        _dbReader = dbReader;
        _validator = validator;
    }

    public async Task<Result<DataListPage<BreedDTO>, ErrorList>> HandleAsync(
        GetBreedsQuery query,
        CancellationToken cancellationToken = default)
    {
        // query validation
        var validatorResult = await _validator.ValidateAsync(query, cancellationToken);

        if (!validatorResult.IsValid)
        {
            var errors = from e in validatorResult.Errors
                         select Error.Deserialize(e.ErrorMessage);
            return new ErrorList(errors);
        }

        return await _dbReader.GetBreedsAsync(query, cancellationToken);
    }
}
