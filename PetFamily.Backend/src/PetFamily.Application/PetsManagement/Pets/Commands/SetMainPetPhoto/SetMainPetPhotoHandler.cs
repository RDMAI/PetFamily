using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Pets.Commands.SetMainPetPhoto;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Pets.Commands.SetMainPetPhoto;
public class SetMainPetPhotoHandler
    : ICommandHandler<PetId, SetMainPetPhotoCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IFileProvider _fileProvider;
    private readonly IUnitOfWork _transactionHelper;
    private readonly SetMainPetPhotoCommandValidator _validator;
    private readonly ILogger<SetMainPetPhotoHandler> _logger;

    public SetMainPetPhotoHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IFileProvider fileProvider,
        IUnitOfWork transactionHelper,
        SetMainPetPhotoCommandValidator validator,
        ILogger<SetMainPetPhotoHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _fileProvider = fileProvider;
        _transactionHelper = transactionHelper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        SetMainPetPhotoCommand command,
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

        var petId = PetId.Create(command.PetId);

        // handling BL
        var domainResult = volunteer.SetMainPetPhoto(petId, command.PhotoPath);
        if (domainResult.IsFailure)
            return domainResult.Error.ToErrorList();

        var dbResult = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
        if (dbResult.IsFailure)
            return dbResult.Error;

        return petId;
    }
}
