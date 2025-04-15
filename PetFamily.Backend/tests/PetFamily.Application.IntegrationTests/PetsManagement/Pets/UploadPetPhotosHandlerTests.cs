using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.Commands.DeletePetPhotos;
using PetFamily.Application.PetsManagement.Pets.Commands.UploadPetPhotos;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.Shared.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class UploadPetPhotosHandlerTests : BaseHandlerTests
{
    public UploadPetPhotosHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_UploadPetPhotosWithValidCommand_ReturnSuccess()
    {
        // Arrange
        _webFactory.SetupSuccessFileProviderMock();

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
        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        using Stream stream = new MemoryStream();

        IEnumerable<FileDTO> fileDTOs = [new FileDTO(
            ContentStream: stream,
            Name: "Test_1")];

        var command = new UploadPetPhotosCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value,
            Photos: fileDTOs);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, UploadPetPhotosCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is still in the database
        var entity = await context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        var photos = entity.Pets.First().Photos.ToList();
        Assert.True(photos.Count == 1, "Photo is not uploaded");
    }
}
