using CSharpFunctionalExtensions;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.SpeciesManagement.Queries.GetSpecies;

public class GetSpeciesHandler
    : IQueryHandler<DataListPage<SpeciesDTO>, GetSpeciesQuery>
{
    private readonly ISpeciesAggregateDBReader _dbReader;
    private readonly GetSpeciesQueryValidator _validator;

    public GetSpeciesHandler(
        ISpeciesAggregateDBReader dbReader,
        GetSpeciesQueryValidator validator)
    {
        _dbReader = dbReader;
        _validator = validator;
    }

    public async Task<Result<DataListPage<SpeciesDTO>, ErrorList>> HandleAsync(
        GetSpeciesQuery query,
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

        return await _dbReader.GetAsync(query, cancellationToken);
    }
}
