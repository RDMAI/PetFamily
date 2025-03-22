using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PetFamily.Application.PetsManagement.Pets.Commands.UploadPetPhotos;
using PetFamily.Application.PetsManagement.Volunteers.Interfaces;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.Shared.Interfaces;
using PetFamily.Application.Shared.Validation;
using PetFamily.Domain.Helpers;
using PetFamily.Domain.PetsManagement.Entities;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared;
using PetFamily.Domain.UnitTests.Helpers;
using System.Data;

namespace PetFamily.Application.UnitTests.PetsManagement;

public class UploadPetPhotosHandlerTests
{
    // these get reinitilized for each test in class
    private readonly Mock<IVolunteerRepository> _volunteerRepositoryMock = new();
    private readonly Mock<IFileProvider> _fileProviderMock = new();
    private readonly Mock<IDbTransaction> _transactionMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly InlineValidator<UploadPetPhotosCommand> _validatorStub = new();
    private readonly NullLogger<UploadPetPhotosHandler> _loggerStub = new();

    [Fact]
    public async Task HandleAsync_AddingValidPhotos_ResultSuccess()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;

        var volunteer = EntitiesHelper.CreateValidVolunteer();
        var pet = EntitiesHelper.CreateValidPet();
        volunteer.AddPet(pet);

        using var stream = new MemoryStream();
        var fileName = "test.jpg";

        var uploadPhotoDTO = new FileDTO(stream, fileName);
        var uploadPhotoData = new FileData(stream, new FileInfoDTO(fileName, Constants.BucketNames.PET_PHOTOS));

        var command = new UploadPetPhotosCommand(
            volunteer.Id.Value,
            pet.Id.Value,
            [uploadPhotoDTO, uploadPhotoDTO]);

        IEnumerable<FileData> filesData = [uploadPhotoData, uploadPhotoData];

        // setting up mocks
        _volunteerRepositoryMock.Setup(d => d.UpdateAsync(volunteer, ct))
            .ReturnsAsync(volunteer.Id);
        _volunteerRepositoryMock.Setup(d => d.GetByIdAsync(volunteer.Id, ct))
            .ReturnsAsync(volunteer);

        _fileProviderMock.Setup(d => d.UploadFilesAsync(filesData, ct))
            .ReturnsAsync(UnitResult.Success<ErrorList>());

        _transactionMock.Setup(d => d.Commit());
        _transactionMock.Setup(d => d.Rollback());
        _transactionMock.Setup(d => d.Dispose());
        _unitOfWorkMock.Setup(d => d.BeginTransaction(ct))
            .ReturnsAsync(_transactionMock.Object);

        var handler = new UploadPetPhotosHandler(
            _volunteerRepositoryMock.Object,
            _fileProviderMock.Object,
            _unitOfWorkMock.Object,
            _validatorStub,
            _loggerStub);

        // Act
        var result = await handler.HandleAsync(
            command,
            ct);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains(volunteer.Pets, p => p.Id == pet.Id);
        Assert.True(pet.Photos.Count != 0);
    }

    [Fact]
    public async Task HandleAsync_WhenInvalidVolunteerId_ResultValidationError()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;

        using var stream = new MemoryStream();
        var fileName = "test.jpg";

        var uploadPhotoDTO = new FileDTO(stream, fileName);

        var command = new UploadPetPhotosCommand(
            Guid.Empty,
            Guid.NewGuid(),
            [uploadPhotoDTO, uploadPhotoDTO]);

        // setting up mocks
        _validatorStub.RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        var handler = new UploadPetPhotosHandler(
            _volunteerRepositoryMock.Object,
            _fileProviderMock.Object,
            _unitOfWorkMock.Object,
            _validatorStub,
            _loggerStub);

        // Act
        var result = await handler.HandleAsync(
            command,
            ct);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Contains(result.Error, e => e.Type == ErrorType.Validation);
    }

    [Fact]
    public async Task HandleAsync_WhenVolunteerNotFound_ResultError()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;

        using var stream = new MemoryStream();
        var fileName = "test.jpg";

        var uploadPhotoDTO = new FileDTO(stream, fileName);

        var volunteerId = VolunteerId.GenerateNew();

        var command = new UploadPetPhotosCommand(
            volunteerId.Value,
            Guid.NewGuid(),
            [uploadPhotoDTO, uploadPhotoDTO]);

        var error = ErrorHelper.General.NotFound();

        // setting up mocks
        _volunteerRepositoryMock.Setup(d => d.GetByIdAsync(volunteerId, ct))
            .ReturnsAsync(Result.Failure<Volunteer, ErrorList>(error));

        var handler = new UploadPetPhotosHandler(
            _volunteerRepositoryMock.Object,
            _fileProviderMock.Object,
            _unitOfWorkMock.Object,
            _validatorStub,
            _loggerStub);

        // Act
        var result = await handler.HandleAsync(
            command,
            ct);

        // Assert
        Assert.True(result.IsFailure);
    }

    [Fact]
    public async Task HandleAsync_WhenVolunteerRepositoryError_ResultError()
    {
        // Arrange
        var ct = new CancellationTokenSource().Token;

        var volunteer = EntitiesHelper.CreateValidVolunteer();
        var pet = EntitiesHelper.CreateValidPet();
        volunteer.AddPet(pet);

        using var stream = new MemoryStream();
        var fileName = "test.jpg";

        var uploadPhotoDTO = new FileDTO(stream, fileName);
        var uploadPhotoData = new FileData(stream, new FileInfoDTO(fileName, Constants.BucketNames.PET_PHOTOS));

        var command = new UploadPetPhotosCommand(
            volunteer.Id.Value,
            pet.Id.Value,
            [uploadPhotoDTO, uploadPhotoDTO]);

        IEnumerable<FileData> filesData = [uploadPhotoData, uploadPhotoData];

        var error = ErrorHelper.Files.UploadFailure();

        // setting up mocks
        _volunteerRepositoryMock.Setup(d => d.UpdateAsync(volunteer, ct))
            .ReturnsAsync(error.ToErrorList());
        _volunteerRepositoryMock.Setup(d => d.GetByIdAsync(volunteer.Id, ct))
            .ReturnsAsync(volunteer);

        _transactionMock.Setup(d => d.Commit());
        _transactionMock.Setup(d => d.Rollback());
        _transactionMock.Setup(d => d.Dispose());
        _unitOfWorkMock.Setup(d => d.BeginTransaction(ct))
            .ReturnsAsync(_transactionMock.Object);

        var handler = new UploadPetPhotosHandler(
            _volunteerRepositoryMock.Object,
            _fileProviderMock.Object,
            _unitOfWorkMock.Object,
            _validatorStub,
            _loggerStub);

        // Act
        var result = await handler.HandleAsync(
            command,
            ct);

        // Assert
        Assert.True(result.IsFailure);
    }
}
