using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;

namespace PetFamily.Application.PetsManagement.Pets.Commands.DeletePet;

public class DeletePetHandler
    : ICommandHandler<PetId, DeletePetCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IDBConnectionFactory _dBConnectionFactory;
    private readonly DeletePetCommandValidator _validator;
    private readonly ILogger<DeletePetHandler> _logger;

    public DeletePetHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IDBConnectionFactory dBConnectionFactory,
        DeletePetCommandValidator validator,
        ILogger<DeletePetHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _dBConnectionFactory = dBConnectionFactory;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        DeletePetCommand command,
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
        var pet = volunteer.Pets.FirstOrDefault(p => p.Id == petId);
        if (pet == null)
            return ErrorHelper.General.NotFound(petId.Value).ToErrorList();

        // mark pet as deleted
        pet.Delete();

        var result = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
        if (result.IsFailure)
            return result.Error;

        return petId;
    }
}
