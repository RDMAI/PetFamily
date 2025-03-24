using CSharpFunctionalExtensions;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;

public class GetVolunteerByIdHandler
    : IQueryHandler<VolunteerDTO, GetVolunteerByIdQuery>
{
    private readonly IVolunteerAggregateDBReader _dbReader;
    private readonly GetVolunteerByIdQueryValidator _validator;

    public GetVolunteerByIdHandler(
        IVolunteerAggregateDBReader dbReader,
        GetVolunteerByIdQueryValidator validator)
    {
        _dbReader = dbReader;
        _validator = validator;
    }

    public async Task<Result<VolunteerDTO, ErrorList>> HandleAsync(
        GetVolunteerByIdQuery query,
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

        return await _dbReader.GetByIdAsync(query, cancellationToken);
    }
}
