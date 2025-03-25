using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.SpeciesManagement.Commands.DeleteSpecies;

public class DeleteSpeciesHandler
    : ICommandHandler<SpeciesId, DeleteSpeciesCommand>
{
    private readonly ISpeciesAggregateRepository _speciesRepository;
    private readonly ISpeciesAggregateDBReader _speciesDBReader;
    private readonly DeleteSpeciesCommandValidator _validator;
    private readonly ILogger<DeleteSpeciesHandler> _logger;

    public DeleteSpeciesHandler(
        ISpeciesAggregateRepository speciesRepository,
        ISpeciesAggregateDBReader speciesDBReader,
        DeleteSpeciesCommandValidator validator,
        ILogger<DeleteSpeciesHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _speciesDBReader = speciesDBReader;
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
        var checkPetsResult = await _speciesDBReader.ArePetsWithSpeciesIdNotExistAsync(speciesId.Value, cancellationToken);
        if (checkPetsResult.IsFailure)  // if error - pets exist
            return checkPetsResult.Error;

        // handle BL
        var response = await _speciesRepository.HardDeleteAsync(entityResult.Value, cancellationToken);

        _logger.LogInformation("Species with id {Id} deleted", speciesId.Value);

        return speciesId;
    }
}
