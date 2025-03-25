using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.SpeciesManagement.Interfaces;
using PetFamily.Domain.Shared;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

namespace PetFamily.Application.SpeciesManagement.Commands.DeleteBreed;

public class DeleteBreedHandler
    : ICommandHandler<BreedId, DeleteBreedCommand>
{
    private readonly ISpeciesAggregateRepository _speciesRepository;
    private readonly ISpeciesAggregateDBReader _speciesDBReader;
    private readonly DeleteBreedCommandValidator _validator;
    private readonly ILogger<DeleteBreedHandler> _logger;

    public DeleteBreedHandler(
        ISpeciesAggregateRepository speciesRepository,
        ISpeciesAggregateDBReader speciesDBReader,
        DeleteBreedCommandValidator validator,
        ILogger<DeleteBreedHandler> logger)
    {
        _speciesRepository = speciesRepository;
        _speciesDBReader = speciesDBReader;
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
        var checkPetsResult = await _speciesDBReader.ArePetsWithBreedIdNotExistAsync(breedId.Value, cancellationToken);
        if (checkPetsResult.IsFailure)  // if error - pets exist
            return checkPetsResult.Error;

        // handle BL
        speciesResult.Value.DeleteBreed(breedId);
        var response = await _speciesRepository.UpdateAsync(speciesResult.Value, cancellationToken);

        _logger.LogInformation("Breed with id {Id} deleted", breedId.Value);

        return breedId;
    }
}
