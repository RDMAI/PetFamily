using Microsoft.Extensions.DependencyInjection;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.SpeciesManagement.Application.DTOs;
using PetFamily.SpeciesManagement.Application.Queries.GetSpecies;
using PetFamily.Tests.Shared;

namespace PetFamily.SpeciesManagement.Application.Tests.SpeciesTests;

public class GetSpeciesHandlerTests : SpeciesManagementBaseTests
{
    private readonly IQueryHandler<DataListPage<SpeciesDTO>, GetSpeciesQuery> _sut;

    public GetSpeciesHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<SpeciesDTO>, GetSpeciesQuery>>();
    }

    [Fact]
    public async Task HandleAsync_GettingPage1_WithPageSize10_WithDescendingSortingByName_ReturnSuccess()
    {
        // Arrange
        // database seeding
        var speciesList = SeedingHelper.CreateValidSpeciesList(12);

        _speciesContext.Species.AddRange(speciesList);
        await _speciesContext.SaveChangesAsync();

        IEnumerable<SortByDTO> sort = [
            new SortByDTO(Property: "name", IsAscending: false)];
        var query = new GetSpeciesQuery(
            Sort: sort,
            CurrentPage: 1,
            PageSize: 10);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(query, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        var data = result.Value.Data;
        Assert.True(data.Any(), "Data from database is empty");
        Assert.True(data.Count() <= 10, "Data pagesize is incorrect");

        var firstId = data.First().Id;
        var firstIdFromManuallyOrdered = data
            .OrderByDescending(v => v.Name)
            .First().Id;
        Assert.True(firstId == firstIdFromManuallyOrdered, "Data ordering is incorrect");
    }
}
