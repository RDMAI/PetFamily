using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;
public class UploadPetPhotosHandler
{
    public const string BUCKET_NAME = "petphotos";

    private readonly IVolunteerRepository _volunteerRepository;
    private readonly IFileProvider _fileProvider;
    private readonly IUnitOfWork _transactionHelper;
    private readonly UploadPetPhotosCommandValidator _validator;
    private readonly ILogger<UploadPetPhotosHandler> _logger;

    public UploadPetPhotosHandler(
        IVolunteerRepository volunteerRepository,
        IFileProvider fileProvider,
        IUnitOfWork transactionHelper,
        UploadPetPhotosCommandValidator validator,
        ILogger<UploadPetPhotosHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _fileProvider = fileProvider;
        _transactionHelper = transactionHelper;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Result<PetId, ErrorList>> HandleAsync(
        UploadPetPhotosCommand command,
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
        var photosList = command.Photos.Select(p =>
            FileVO.Create(
                Guid.NewGuid().ToString(),
                p.Name).Value);

        // handling BL
        var transaction = await _transactionHelper.BeginTransaction(cancellationToken);
        try
        {
            // save photos' paths to DB
            volunteer.AddPhotosToPet(petId, photosList);
            var dbResult = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
            if (dbResult.IsFailure)
            {
                transaction.Rollback();
                return dbResult.Error;
            }

            // save photos to file storage
            var filesData = command.Photos.Select(p => new FileData(p.ContentStream, p.Name, BUCKET_NAME));
            var fileStorageResult = await _fileProvider.UploadFilesAsync(filesData, cancellationToken);
            if (fileStorageResult.IsFailure)
            {
                transaction.Rollback();
                return fileStorageResult.Error;
            }

            transaction.Commit();

            return petId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Cannot add photos to pet with id = {id}. Rolling back transaction.",
                command.PetId);
            transaction.Rollback();

            return Error.Failure("Internal.error", "Cannot add photos to pet.").ToErrorList();
        }
    }
}
