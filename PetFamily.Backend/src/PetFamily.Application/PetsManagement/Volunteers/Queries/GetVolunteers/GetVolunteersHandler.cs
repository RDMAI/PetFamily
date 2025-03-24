﻿using CSharpFunctionalExtensions;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;

public class GetVolunteersHandler
    : IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery>
{
    private readonly IVolunteerAggregateDBReader _dbReader;
    private readonly GetVolunteersQueryValidator _validator;

    public GetVolunteersHandler(
        IVolunteerAggregateDBReader dbReader,
        GetVolunteersQueryValidator validator)
    {
        _dbReader = dbReader;
        _validator = validator;
    }

    public async Task<Result<DataListPage<VolunteerDTO>, ErrorList>> HandleAsync(
        GetVolunteersQuery query,
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
