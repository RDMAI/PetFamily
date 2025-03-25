using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using System.Text;

namespace PetFamily.Application.SpeciesManagement.Commands.DeleteBreed;

public class DeleteBreedHandler
    : ICommandHandler<BreedId, DeleteBreedCommand>
{
    private readonly ISpeciesAggregateRepository _speciesRepository;
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly DeleteBreedCommandValidator _validator;
    private readonly ILogger<DeleteBreedHandler> _logger;

    public DeleteBreedHandler(
        ISpeciesAggregateRepository speciesRepository,
        IDBConnectionFactory dBConnectionFactory,
        DeleteBreedCommandValidator validator,
        ILogger<DeleteBreedHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<BreedId, ErrorList>> HandleAsync(
        DeleteBreedCommand command,
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
        var speciesResult = await _speciesRepository.GetByIdAsync(speciesId, cancellationToken);
        if (speciesResult.IsFailure)
            return speciesResult.Error;

        // check if there are any pet of this breed
        var breedId = BreedId.Create(command.BreedId);
        var checkPetsResult = await ArePetsWithBreedIdNotExistAsync(breedId.Value, cancellationToken);
        if (checkPetsResult.IsFailure)  // if error - pets exist
            return checkPetsResult.Error;

        // handle BL
        speciesResult.Value.DeleteBreed(breedId);
        var response = await _speciesRepository.UpdateAsync(speciesResult.Value, cancellationToken);

        _logger.LogInformation("Breed with id {Id} deleted", breedId.Value);

        return breedId;
    }

    public async Task<UnitResult<ErrorList>> ArePetsWithBreedIdNotExistAsync(
        Guid BreedId,
        CancellationToken cancellationToken = default)
    {
        using var connection = _dBConnectionFactory.Create();

        var parameters = new DynamicParameters();
        parameters.Add("@breedId", BreedId);

        var sql = new StringBuilder(
            """
            SELECT id
            FROM Pets
            WHERE breed_id = @breedId
            LIMIT 1
            """
        );

        var result = await connection.QueryAsync<Guid>(sql.ToString(), parameters);
        if (result is not null && result.Any())
            return Error.Conflict(
                "relation.exist",
                $"Cannot delete breed {BreedId}. It has related pet: {result.First()}")
                .ToErrorList();

        return UnitResult.Success<ErrorList>();
    }
}
