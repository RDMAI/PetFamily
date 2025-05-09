﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Pets.Commands.DeletePet;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Pets;

public class DeletePetHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<PetId, DeletePetCommand> sut;

    public DeletePetHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<PetId, DeletePetCommand>>();
    }

    [Fact]
    public async Task HandleAsync_SoftDeletingPetWithoutPhotosWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);
        volunteer.AddPet(pet);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var command = new DeletePetCommand(
            VolunteerId: volunteer.Id.Value,
            PetId: pet.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is soft deleted in the database
        var entity = await _petsContext.Volunteers
            .Where(v => v.Pets.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is not null, "Pet is not in the database");
        Assert.True(entity.Pets.Any(p => p.Id == result.Value && p.IsDeleted), "Pet is not softdeleted");
    }

    [Fact]
    public async Task HandleAsync_TryingToDeleteFromNonexistantVolunteer_ReturnError()
    {
        // Arrange
        // seed database
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        var species = SeedingHelper.CreateValidSpecies("Dog");
        species.AddBreed(breed);
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pet = SeedingHelper.CreateValidPet(breed.Id, species.Id);
        volunteer.AddPet(pet);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var fakeVolunteerId = VolunteerId.GenerateNew();

        var command = new DeletePetCommand(
            VolunteerId: fakeVolunteerId.Value,
            PetId: pet.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure, "Returned success, expected failure");
        Assert.True(result.Error is not null, "Result error is null");

        // check if the entity is NOT in the database
        var entity = await _petsContext.Volunteers
            .Where(v => v.Pets.Any(p => p.IsDeleted)).FirstOrDefaultAsync();
        Assert.True(entity is null, "Entity is not deleted, expected otherwise");
    }
}
