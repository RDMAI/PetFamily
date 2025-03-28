﻿using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.Shared.Messaging;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.PetsManagement.Pets.Commands.UploadPetPhotos;
public class UploadPetPhotosHandler
    : ICommandHandler<PetId, UploadPetPhotosCommand>
{
    private readonly IVolunteerAggregateRepository _volunteerRepository;
    private readonly IFileProvider _fileProvider;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMessageQueue<IEnumerable<FileInfoDTO>> _fileMessageQueue;
    private readonly IValidator<UploadPetPhotosCommand> _validator;
    private readonly ILogger<UploadPetPhotosHandler> _logger;

    public UploadPetPhotosHandler(
        IVolunteerAggregateRepository volunteerRepository,
        IFileProvider fileProvider,
        IUnitOfWork unitOfWork,
        IMessageQueue<IEnumerable<FileInfoDTO>> fileMessageQueue,
        IValidator<UploadPetPhotosCommand> validator,
        ILogger<UploadPetPhotosHandler> logger)
    {
        _volunteerRepository = volunteerRepository;
        _fileProvider = fileProvider;
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
        IEnumerable<FileVO> photosToDatabase = [];
        IEnumerable<FileData> photosToStorage = [];
        foreach (var file in command.Photos)
        {
            var storageName = Guid.NewGuid().ToString() + "_" + file.Name;

            photosToDatabase = photosToDatabase.Append(FileVO.Create(storageName, file.Name).Value);
            photosToStorage = photosToStorage.Append(new FileData(
                file.ContentStream,
                new FileInfoDTO(storageName, Constants.BucketNames.PET_PHOTOS)));
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
            var fileStorageResult = await _fileProvider.UploadFilesAsync(photosToStorage, cancellationToken);
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
