using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.Commands.SetMainPetPhoto;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class SetMainPetPhotoHandlerTests : BaseHandlerTests
{
    public SetMainPetPhotoHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_SettingNewMainPhotoWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        context.Species.Add(species);

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);
        volunteer.AddPet(pet);

        List<string> pathsToFiles = [
            Guid.NewGuid().ToString() + "_test1.jpg",
            Guid.NewGuid().ToString() + "_test2.jpg"];
        var files = pathsToFiles.Select(f => FileVO.Create(f, "test.jpg").Value);
        volunteer.AddPhotosToPet(pet.Id, files);

        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        var command = new SetMainPetPhotoCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value,
            PhotoPath: pathsToFiles[1]);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, SetMainPetPhotoCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.Pets is not null, "Pets list is null");
        Assert.True(entity.Pets.Count == 1, "Pets list count is incorrect");

        var petFromDB = entity.Pets.First();
        Assert.True(petFromDB.Photos.Count == 2, "Pet photos amount is changed");

        var mainPhotoFromDB = petFromDB.Photos[0].PathToStorage;
        Assert.True(mainPhotoFromDB == command.PhotoPath, "Main photo is incorrect");
    }
}
