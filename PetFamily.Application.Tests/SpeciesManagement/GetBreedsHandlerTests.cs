using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.SpeciesManagement;

public class GetBreedsHandlerTests : BaseHandlerTests
{
    public GetBreedsHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_GettingPage1_WithPageSize10_WithDescendingSortingByName_ReturnSuccess()
    {
        // Arrange
        // database seeding
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        var dog = SeedingHelper.CreateValidSpecies("Dog");
        var cat = SeedingHelper.CreateValidSpecies("Cat");
        var breedList = SeedingHelper.CreateValidBreedList(13);

        // add the last breed to cat and the rest - to dog
        for (int i = 0; i < breedList.Count - 2; i++)
            dog.AddBreed(breedList[i]);
        cat.AddBreed(breedList.Last());

        context.Species.Add(dog);
        context.Species.Add(cat);

        await context.SaveChangesAsync();

        IEnumerable<SortByDTO> sort = [
            new SortByDTO(Property: "name", IsAscending: false)];
        var query = new GetBreedsQuery(
            Sort: sort,
            CurrentPage: 1,
            PageSize: 10,
            SpeciesId: dog.Id.Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<BreedDTO>, GetBreedsQuery>>();

        // Act
        var result = await sut.HandleAsync(query, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        var data = result.Value.Data;
        Assert.True(data.Any(), "Data from database is empty");
        Assert.True(data.Count() <= 10, "Data pagesize is incorrect");
        Assert.Contains(data, b => b.Species_Id != cat.Id.Value);
        
        var firstId = data.First().Id;
        var firstIdFromManuallyOrdered = data
            .OrderByDescending(v => v.Name)
            .First().Id;
        Assert.True(firstId == firstIdFromManuallyOrdered, "Data ordering is incorrect");
    }
}
