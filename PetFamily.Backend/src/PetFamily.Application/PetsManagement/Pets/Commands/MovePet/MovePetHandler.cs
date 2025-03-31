using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Pets.Commands.MovePet;

public class MovePetHandler
    : ICommandHandler<PetId, MovePetCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly MovePetCommandValidator _validator;
    private readonly ILogger<MovePetHandler> _logger;

    public MovePetHandler(
        IVolunteerAggregateRepository volunteerRepository,
        MovePetCommandValidator validator,
        ILogger<MovePetHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        MovePetCommand command,
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

        // validate volunteer
        var volunteerId = VolunteerId.Create(command.VolunteerId);
        var volunteerResult = await _volunteerRepository.GetByIdAsync(volunteerId, cancellationToken);
        if (volunteerResult.IsFailure)
            return volunteerResult.Error;
        var volunteer = volunteerResult.Value;

        // validate pet
        var petId = PetId.Create(command.PetId);
        if (volunteer.Pets.Any(p => p.Id == petId) == false)
            return ErrorHelper.General.NotFound(command.PetId).ToErrorList();

        var newSerialNumber = PetSerialNumber.Create(command.NewSerialNumber).Value;

        var domainResult = volunteer.MovePet(petId, newSerialNumber);
        if (domainResult.IsFailure)
            return domainResult.Error.ToErrorList();

        var result = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return petId;
    }
}
