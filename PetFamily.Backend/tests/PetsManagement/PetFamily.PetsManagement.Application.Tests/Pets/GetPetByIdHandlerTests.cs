using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.PetsManagement.Application.Pets.Queries.GetPetById;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Pets;

public class GetPetByIdHandlerTests : PetsManagementBaseTests
{
    private readonly IQueryHandler<PetDTO, GetPetByIdQuery> _sut;

    public GetPetByIdHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PetDTO, GetPetByIdQuery>>();
    }

    [Fact]
    public async Task HandleAsync_GettingPetByIdWithValidQuery_ReturnSuccess()
    {
        // Arrange
        // database seeding
        var species = SeedingHelper.CreateValidSpecies("Dog");
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        species.AddBreed(breed);
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(
            speciesId: species.Id,
            breedId: breed.Id,
            amount: 2);
        foreach (var pet in pets)
            volunteer.AddPet(pet);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var query = new GetPetByIdQuery(volunteer.Id.Value, pets.First().Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(query, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");
        Assert.True(result.Value.Id == query.PetId, "Entity with wrong id returned");
    }

    [Fact]
    public async Task HandleAsync_TryingToGetNonexistantPet_ReturnError()
    {
        // Arrange
        // database seeding
        var species = SeedingHelper.CreateValidSpecies("Dog");
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        species.AddBreed(breed);
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(
            speciesId: species.Id,
            breedId: breed.Id,
            amount: 2);
        foreach (var pet in pets)
            volunteer.AddPet(pet);
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var query = new GetPetByIdQuery(volunteer.Id.Value, PetId.GenerateNew().Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsFailure, "Returned success, expected failure");
        Assert.True(result.Error is not null, "Result error is null");
    }
}
