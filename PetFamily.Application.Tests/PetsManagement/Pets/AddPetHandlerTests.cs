using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.Commands.AddPet;
using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class AddPetHandlerTests : BaseHandlerTests
{
    private readonly ICommandHandler<PetId, AddPetCommand> sut;

    public AddPetHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, AddPetCommand>>();
    }

    private static AddPetCommand _createCommand(
        VolunteerId volunteerId,
        BreedId breedId)
    {
        var petDTO = new AddPetDTO(
            Name: "Fido",
            Description: "test description",
            Color: "brown",
            Weight: 8,
            Height: 0.3f,
            BreedId: breedId.Value,
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

        return new AddPetCommand(
            VolunteerId: volunteerId.Value,
            Pet: petDTO,
            Address: addressDTO,
            RequisitesList: [ new RequisitesDTO(
                Name: "SPB",
                Description: "Test SPB description",
                Value: "89000000000")]);
    }

    [Fact]
    public async Task HandleAsync_AddingPetToVolunteerWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        _context.Species.Add(species);

        var volunteer = SeedingHelper.CreateValidVolunteer();
        _context.Volunteers.Add(volunteer);

        await _context.SaveChangesAsync();

        var command = _createCommand(volunteer.Id, breed.Id);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await _context.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.Pets is not null, "Pets list is null");
        Assert.True(entity.Pets.Count == 1, "Pet added multiple times");
    }

    [Fact]
    public async Task HandleAsync_AddingPetToNonexistantVolunteer_ReturnError()
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

        var fakeVolunteerId = VolunteerId.GenerateNew();

        var ct = new CancellationTokenSource().Token;

        var command = _createCommand(fakeVolunteerId, breed.Id);

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure, "Returned success, expected failure");
        Assert.True(result.Error is not null, "Result error is null");

        // check if the entity is NOT in the database
        var entity = await context.Volunteers
            .Where(v => v.Pets.Any()).FirstOrDefaultAsync();
        Assert.True(entity is null, "Entity from database is not null, expected otherwise");
    }

    [Fact]
    public async Task HandleAsync_AddingPetToNonexistantBreed_ReturnError()
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

        var fakeBreedId = BreedId.GenerateNew();

        var ct = new CancellationTokenSource().Token;

        var command = _createCommand(volunteer.Id, fakeBreedId);

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure, "Returned success, expected failure");
        Assert.True(result.Error is not null, "Result error is null");

        // check if the entity is NOT in the database
        var entity = await context.Volunteers
            .Where(v => v.Pets.Any()).FirstOrDefaultAsync();
        Assert.True(entity is null, "Entity from database is not null, expected otherwise");
    }
}
