using Microsoft.Extensions.DependencyInjection;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.SpeciesManagement.Application.DTOs;
using PetFamily.SpeciesManagement.Application.Queries.GetBreeds;
using PetFamily.Tests.Shared;

namespace PetFamily.SpeciesManagement.Application.Tests.Breeds;

public class GetBreedsHandlerTests : SpeciesManagementBaseTests
{
    private readonly IQueryHandler<DataListPage<BreedDTO>, GetBreedsQuery> _sut;

    public GetBreedsHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<BreedDTO>, GetBreedsQuery>>();
    }

    [Fact]
    public async Task HandleAsync_GettingPage1_WithPageSize10_WithDescendingSortingByName_ReturnSuccess()
    {
        // Arrange
        // database seeding
        var dog = SeedingHelper.CreateValidSpecies("Dog");
        var cat = SeedingHelper.CreateValidSpecies("Cat");
        var breedList = SeedingHelper.CreateValidBreedList(13);

        // add the last breed to cat and the rest - to dog
        for (int i = 0; i < breedList.Count - 2; i++)
            dog.AddBreed(breedList[i]);
        cat.AddBreed(breedList.Last());

        _speciesContext.Species.Add(dog);
        _speciesContext.Species.Add(cat);
        await _speciesContext.SaveChangesAsync();

        IEnumerable<SortByDTO> sort = [
            new SortByDTO(Property: "name", IsAscending: false)];
        var query = new GetBreedsQuery(
            Sort: sort,
            CurrentPage: 1,
            PageSize: 10,
            SpeciesId: dog.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(query, ct);

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
