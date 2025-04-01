using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.Commands.DeletePetPhotos;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.Shared.ValueObjects;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class DeletePetPhotosHandlerTests : BaseHandlerTests
{
    private readonly ICommandHandler<PetId, DeletePetPhotosCommand> sut;

    public DeletePetPhotosHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, DeletePetPhotosCommand>>();
    }

    [Fact]
    public async Task HandleAsync_DeletingPetPhotosWithValidCommand_ReturnSuccess()
    {
        // Arrange
        _webFactory.SetupSuccessFileProviderMock();

        // seed database
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        _context.Species.Add(species);

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);
        
        var pathToFile = Guid.NewGuid().ToString() + "_test.jpg";
        IEnumerable<FileVO> files = [FileVO.Create(pathToFile, "test.jpg").Value];
        volunteer.AddPhotosToPet(pet.Id, files);

        volunteer.AddPet(pet);
        _context.Volunteers.Add(volunteer);

        await _context.SaveChangesAsync();

        var command = new DeletePetPhotosCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value,
            PhotoPaths: [pathToFile]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is still in the database
        var entity = await _context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        var photos = entity.Pets.First().Photos.ToList();
        Assert.True(photos.Count == 0, "Photo is not deleted");
    }

    [Fact]
    public async Task HandleAsync_TryingToDeletePhotosFromNonexistantVolunteer_ReturnSuccess()
    {
        // Arrange
        _webFactory.SetupSuccessFileProviderMock();

        // seed database
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        _context.Species.Add(species);

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);

        var pathToFile = Guid.NewGuid().ToString() + "_test.jpg";
        IEnumerable<FileVO> files = [FileVO.Create(pathToFile, "test.jpg").Value];
        volunteer.AddPhotosToPet(pet.Id, files);

        volunteer.AddPet(pet);
        _context.Volunteers.Add(volunteer);

        await _context.SaveChangesAsync();

        var fakeVolunteerId = VolunteerId.GenerateNew();

        var command = new DeletePetPhotosCommand(
            VolunteerId: fakeVolunteerId.Value,
            PetId: pet.Id.Value,
            PhotoPaths: [pathToFile]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure, "Returned success, expected failure");
        Assert.True(result.Error is not null, "Result error is null");

        // check if the entity is in the database
        var entity = await _context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        var photos = entity.Pets.First().Photos.ToList();
        Assert.True(photos.Count != 0, "Photo is not deleted");
    }
}
