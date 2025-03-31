using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.Commands.AddPet;
using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class AddPetHandlerTests : BaseHandlerTests
{
    public AddPetHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_AddingPetToVolunteerWithValidCommand_ReturnSuccess()
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
        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        var petDTO = new AddPetDTO(
            Name: "Fido",
            Description: "test description",
            Color: "brown",
            Weight: 8,
            Height: 0.3f,
            BreedId: breed.Id.Value,
            HealthInformation: "Healthy",
            OwnerPhone: "89000000000",
            IsCastrated: true,
            BirthDate: new DateOnly(2025, 1, 1),
            IsVacinated: true,
            Status: (int)PetStatuses.FoundHome);

        var addressDTO = new AddressDTO(
            City: "Moscow",
            Street: "test street",
            HouseNumber: 1,
            AppartmentNumber: 1);

        var command = new AddPetCommand(
            VolunteerId: volunteer.Id.Value,
            Pet: petDTO,
            Address: addressDTO,
            RequisitesList: [ new RequisitesDTO(
                Name: "SPB",
                Description: "Test SPB description",
                Value: "89000000000")]);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, AddPetCommand>>();

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
        Assert.True(entity.Pets.Count == 1, "Pet added multiple times");
    }
}
