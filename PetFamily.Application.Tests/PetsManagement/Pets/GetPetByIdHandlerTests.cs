using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Pets.DTOs;
using PetFamily.Application.PetsManagement.Pets.Queries.GetPetById;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.PetsManagement.ValueObjects.Pets;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Pets;

public class GetPetByIdHandlerTests : BaseHandlerTests
{
    private readonly IQueryHandler<PetDTO, GetPetByIdQuery> sut;

    public GetPetByIdHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PetDTO, GetPetByIdQuery>>();
    }

    [Fact]
    public async Task HandleAsync_GettingPetByIdWithValidQuery_ReturnSuccess()
    {
        // Arrange
        // database seeding
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        var species = SeedingHelper.CreateValidSpecies("Dog");
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        species.AddBreed(breed);

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(
            speciesId: species.Id,
            breedId: breed.Id,
            amount: 2);
        foreach (var pet in pets)
            volunteer.AddPet(pet);

        context.Volunteers.Add(volunteer);
        await context.SaveChangesAsync();

        var query = new GetPetByIdQuery(volunteer.Id.Value, pets.First().Id.Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PetDTO, GetPetByIdQuery>>();

        // Act
        var result = await sut.HandleAsync(query, ct);

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
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        var species = SeedingHelper.CreateValidSpecies("Dog");
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        species.AddBreed(breed);

        var volunteer = SeedingHelper.CreateValidVolunteer();
        var pets = SeedingHelper.CreateValidPetList(
            speciesId: species.Id,
            breedId: breed.Id,
            amount: 2);
        foreach (var pet in pets)
            volunteer.AddPet(pet);

        context.Volunteers.Add(volunteer);
        await context.SaveChangesAsync();

        var query = new GetPetByIdQuery(volunteer.Id.Value, PetId.GenerateNew().Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<IQueryHandler<PetDTO, GetPetByIdQuery>>();

        // Act
        var result = await sut.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsFailure, "Returned success, expected failure");
        Assert.True(result.Error is not null, "Result error is null");
    }
}
