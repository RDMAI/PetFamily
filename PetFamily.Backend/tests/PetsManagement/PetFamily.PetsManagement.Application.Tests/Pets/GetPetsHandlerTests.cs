﻿using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Pets.DTOs;
using PetFamily.PetsManagement.Application.Pets.Queries.GetPets;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Pets;

public class GetPetsHandlerTests : PetsManagementBaseTests
{
    private readonly IQueryHandler<DataListPage<PetDTO>, GetPetsQuery> _sut;

    public GetPetsHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<PetDTO>, GetPetsQuery>>();
    }

    [Fact]
    public async Task HandleAsync_GettingPets_WithPaging_WithSorting_WithFiltering_ReturnSuccess()
    {
        // Arrange
        // database seeding
        var dog = SeedingHelper.CreateValidSpecies("Dog");
        var cat = SeedingHelper.CreateValidSpecies("Cat");
        var labrador = SeedingHelper.CreateValidBreed("Labrador");
        var shepherd = SeedingHelper.CreateValidBreed("Shepherd");
        dog.AddBreed(labrador);
        dog.AddBreed(shepherd);

        _speciesContext.Species.AddRange([dog, cat]);
        await _speciesContext.SaveChangesAsync();

        var volunteers = SeedingHelper.CreateValidVolunteerList(2);
        var labradorsPetList = SeedingHelper.CreateValidPetList(
            speciesId: dog.Id,
            breedId: labrador.Id,
            amount: 30);
        var shepherdPet = SeedingHelper.CreateValidPet(
            specificName: "Shepherd",
            speciesId: dog.Id,
            breedId: shepherd.Id);
        var additionalShepherd = SeedingHelper.CreateValidPet(
            specificName: "Add Shepherd",
            speciesId: dog.Id,
            breedId: shepherd.Id);

        var volunteerMain = volunteers.First();
        var volunteerAdd = volunteers.Last();

        foreach (var labr in labradorsPetList)
            volunteerMain.AddPet(labr);
        volunteerMain.AddPet(shepherdPet);
        volunteerAdd.AddPet(additionalShepherd);

        _petsContext.Volunteers.AddRange([volunteerMain, volunteerAdd]);
        await _petsContext.SaveChangesAsync();

        var query = new GetPetsQuery(
            CurrentPage: 1,
            PageSize: 5,
            Sort: [new SortByDTO(Property: "name", IsAscending: false)],
            Volunteer_Id: volunteerMain.Id.Value,
            Breed_Id: labrador.Id.Value,
            MaxHeight: 100,
            MaxWeight: 20,
            MaxAge: 10
            );

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(query, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");
    }
}
