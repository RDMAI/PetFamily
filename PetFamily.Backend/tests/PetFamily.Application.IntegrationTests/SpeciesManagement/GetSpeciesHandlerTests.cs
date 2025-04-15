using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;
using PetFamily.Application.SpeciesManagement.Queries.GetSpecies;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.SpeciesManagement;

public class GetSpeciesHandlerTests : BaseHandlerTests
{
    public GetSpeciesHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_GettingPage1_WithPageSize10_WithDescendingSortingByName_ReturnSuccess()
    {
        // Arrange
        // database seeding
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        var speciesList = SeedingHelper.CreateValidSpeciesList(12);

        context.Species.AddRange(speciesList);

        await context.SaveChangesAsync();

        IEnumerable<SortByDTO> sort = [
            new SortByDTO(Property: "name", IsAscending: false)];
        var query = new GetSpeciesQuery(
            Sort: sort,
            CurrentPage: 1,
            PageSize: 10);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<SpeciesDTO>, GetSpeciesQuery>>();

        // Act
        var result = await sut.HandleAsync(query, ct);

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
