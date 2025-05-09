﻿using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Application.Interfaces;
using System.Text;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.SpeciesManagement.Application.Commands.DeleteSpecies;

public class DeleteSpeciesHandler
    : ICommandHandler<SpeciesId, DeleteSpeciesCommand>
{
    private readonly ISpeciesAggregateRepository _speciesRepository;
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly DeleteSpeciesCommandValidator _validator;
    private readonly ILogger<DeleteSpeciesHandler> _logger;

    public DeleteSpeciesHandler(
        ISpeciesAggregateRepository speciesRepository,
        [FromKeyedServices(DependencyKey.Species)] IDBConnectionFactory dBConnectionFactory,
        DeleteSpeciesCommandValidator validator,
        ILogger<DeleteSpeciesHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<SpeciesId, ErrorList>> HandleAsync(
        DeleteSpeciesCommand command,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);

        if (!validatorResult.IsValid)
        {
            var errors = from e in validatorResult.Errors
                         select Error.Deserialize(e.ErrorMessage);
            return new ErrorList(errors);
        }

        // check if species exist
        var speciesId = SpeciesId.Create(command.SpeciesId);
        var entityResult = await _speciesRepository.GetByIdAsync(speciesId, cancellationToken);
        if (entityResult.IsFailure)
            return entityResult.Error;

        // check if there are any pet of this species
        var checkPetsResult = await ArePetsWithSpeciesIdNotExistAsync(speciesId.Value, cancellationToken);
        if (checkPetsResult.IsFailure)  // if error - pets exist
            return checkPetsResult.Error;

        // handle BL
        var response = await _speciesRepository.HardDeleteAsync(entityResult.Value, cancellationToken);

        _logger.LogInformation("Species with id {Id} deleted", speciesId.Value);

        return speciesId;
    }

    public async Task<UnitResult<ErrorList>> ArePetsWithSpeciesIdNotExistAsync(
        Guid SpeciesId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@speciesId", SpeciesId);

        var sql = new StringBuilder(
            """
            SELECT id
            FROM pets_management.Pets
            WHERE species_id = @speciesId
            """
        );

        var result = await connection.QueryAsync<Guid?>(sql.ToString(), parameters);
        if (result is not null && result.Any())
            return Error.Conflict(
                "relation.exist",
                $"Cannot delete species {SpeciesId}. It has related pet: {result.First()}")
                .ToErrorList();

        return UnitResult.Success<ErrorList>();
    }
}
