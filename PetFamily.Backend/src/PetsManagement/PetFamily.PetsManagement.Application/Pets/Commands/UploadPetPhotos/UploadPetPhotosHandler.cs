using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Files.Contracts;
using PetFamily.Files.Contracts.Requests;
using PetFamily.PetsManagement.Application.Volunteers.Interfaces;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.Files;
using PetFamily.Shared.Core.Messaging;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.PetsManagement.Application.Pets.Commands.UploadPetPhotos;
public class UploadPetPhotosHandler
    : ICommandHandler<PetId, UploadPetPhotosCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IFileContract _fileAPI;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueue<IEnumerable<Shared.Core.Files.FileInfo>> _fileMessageQueue;
    private readonly IValidator<UploadPetPhotosCommand> _validator;
    private readonly ILogger<UploadPetPhotosHandler> _logger;

    public UploadPetPhotosHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IFileContract fileAPI,
        [FromKeyedServices(DependencyKey.Pets)] IUnitOfWork unitOfWork,
        IMessageQueue<IEnumerable<Shared.Core.Files.FileInfo>> fileMessageQueue,
        IValidator<UploadPetPhotosCommand> validator,
        ILogger<UploadPetPhotosHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _fileAPI = fileAPI;
        _unitOfWork = unitOfWork;
        _fileMessageQueue = fileMessageQueue;
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

        // create lists of photos to store in file storage and in database
        IEnumerable<Shared.Kernel.ValueObjects.File> photosToDatabase = [];
        IEnumerable<FileData> photosToStorage = [];
        foreach (var file in command.Photos)
        {
            var storageName = Guid.NewGuid().ToString() + "_" + file.NameWithExtenson;

            photosToDatabase = photosToDatabase.Append(Shared.Kernel.ValueObjects.File.Create(storageName, file.NameWithExtenson).Value);
            photosToStorage = photosToStorage.Append(new FileData(
                file.ContentStream,
                new Shared.Core.Files.FileInfo(storageName, Constants.BucketNames.PET_PHOTOS)));
        }

        // handling BL
        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);
        try
        {
            // save photos' paths to DB
            var domainResult = volunteer.AddPhotosToPet(petId, photosToDatabase);
            if (domainResult.IsFailure)
                return domainResult.Error.ToErrorList();

            var dbResult = await _volunteerRepository.UpdateAsync(volunteer, cancellationToken);
            if (dbResult.IsFailure)
            {
                transaction.Rollback();
                return dbResult.Error;
            }

            // save photos to file storage
            var fileStorageResult = await _fileAPI.UploadFilesAsync(
                request: new UploadFilesRequest(photosToStorage),
                cancellationToken);
            if (fileStorageResult.IsFailure)
            {
                // Placing an operation to delete invalid files to memory queue
                var fileInfos = photosToStorage.Select(f => f.Info).ToList();
                await _fileMessageQueue.WriteAsync(fileInfos, cancellationToken);

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

            return ErrorHelper.Files.UploadFailure("Cannot add photos to pet.").ToErrorList();
        }
    }
}
