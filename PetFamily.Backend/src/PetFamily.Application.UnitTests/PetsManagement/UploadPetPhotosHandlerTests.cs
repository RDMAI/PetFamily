using CSharpFunctionalExtensions;
using FluentValidation;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using PetFamily.Application.PetsManagement.Pets.UploadPetPhotos;
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
        var uploadPhotoData = new FileData(stream, fileName, Constants.BucketNames.PET_PHOTOS);

        var command = new UploadPetPhotosCommand(
            volunteer.Id.Value,
            pet.Id.Value,
            [uploadPhotoDTO, uploadPhotoDTO]);

        IEnumerable<FileData> filesData = [uploadPhotoData, uploadPhotoData];

        // mocking dependencies
        var volunteerRepositoryMock = new Mock<IVolunteerRepository>();
        volunteerRepositoryMock.Setup(d => d.UpdateAsync(volunteer, ct))
            .ReturnsAsync(volunteer.Id);
        volunteerRepositoryMock.Setup(d => d.GetByIdAsync(volunteer.Id, ct))
            .ReturnsAsync(volunteer);

        var fileProviderMock = new Mock<IFileProvider>();
        fileProviderMock.Setup(d => d.UploadFilesAsync(filesData, ct))
            .ReturnsAsync(UnitResult.Success<ErrorList>());

        var transactionMock = new Mock<IDbTransaction>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        transactionMock.Setup(d => d.Commit());
        transactionMock.Setup(d => d.Rollback());
        transactionMock.Setup(d => d.Dispose());
        unitOfWorkMock.Setup(d => d.BeginTransaction(ct))
            .ReturnsAsync(transactionMock.Object);

        var validatorStub = new InlineValidator<UploadPetPhotosCommand>();
        var loggerStub = new NullLogger<UploadPetPhotosHandler>();

        var handler = new UploadPetPhotosHandler(
            volunteerRepositoryMock.Object,
            fileProviderMock.Object,
            unitOfWorkMock.Object,
            validatorStub,
            loggerStub);

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

        // mocking dependencies
        var volunteerRepositoryMock = new Mock<IVolunteerRepository>();
        var fileProviderMock = new Mock<IFileProvider>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var validatorStub = new InlineValidator<UploadPetPhotosCommand>();
        validatorStub.RuleFor(c => c.VolunteerId)
            .NotEmpty()
            .WithError(ErrorHelper.General.ValueIsNullOrEmpty("VolunteerId"));

        var loggerStub = new NullLogger<UploadPetPhotosHandler>();

        var handler = new UploadPetPhotosHandler(
            volunteerRepositoryMock.Object,
            fileProviderMock.Object,
            unitOfWorkMock.Object,
            validatorStub,
            loggerStub);

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

        // mocking dependencies
        var volunteerRepositoryMock = new Mock<IVolunteerRepository>();
        volunteerRepositoryMock.Setup(d => d.GetByIdAsync(volunteerId, ct))
            .ReturnsAsync(Result.Failure<Volunteer, ErrorList>(error));

        var fileProviderMock = new Mock<IFileProvider>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        var validatorStub = new InlineValidator<UploadPetPhotosCommand>();
        var loggerStub = new NullLogger<UploadPetPhotosHandler>();

        var handler = new UploadPetPhotosHandler(
            volunteerRepositoryMock.Object,
            fileProviderMock.Object,
            unitOfWorkMock.Object,
            validatorStub,
            loggerStub);

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
        var uploadPhotoData = new FileData(stream, fileName, Constants.BucketNames.PET_PHOTOS);

        var command = new UploadPetPhotosCommand(
            volunteer.Id.Value,
            pet.Id.Value,
            [uploadPhotoDTO, uploadPhotoDTO]);

        IEnumerable<FileData> filesData = [uploadPhotoData, uploadPhotoData];

        var error = ErrorHelper.Files.UploadFailure();

        // mocking dependencies
        var volunteerRepositoryMock = new Mock<IVolunteerRepository>();
        volunteerRepositoryMock.Setup(d => d.UpdateAsync(volunteer, ct))
            .ReturnsAsync(error.ToErrorList());
        volunteerRepositoryMock.Setup(d => d.GetByIdAsync(volunteer.Id, ct))
            .ReturnsAsync(volunteer);

        var fileProviderMock = new Mock<IFileProvider>();

        var transactionMock = new Mock<IDbTransaction>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        transactionMock.Setup(d => d.Commit());
        transactionMock.Setup(d => d.Rollback());
        transactionMock.Setup(d => d.Dispose());
        unitOfWorkMock.Setup(d => d.BeginTransaction(ct))
            .ReturnsAsync(transactionMock.Object);

        var validatorStub = new InlineValidator<UploadPetPhotosCommand>();
        var loggerStub = new NullLogger<UploadPetPhotosHandler>();

        var handler = new UploadPetPhotosHandler(
            volunteerRepositoryMock.Object,
            fileProviderMock.Object,
            unitOfWorkMock.Object,
            validatorStub,
            loggerStub);

        // Act
        var result = await handler.HandleAsync(
            command,
            ct);

        // Assert
        Assert.True(result.IsFailure);
    }
}
