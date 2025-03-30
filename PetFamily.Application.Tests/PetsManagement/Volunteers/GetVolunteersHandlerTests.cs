using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Volunteers;

public class GetVolunteersHandlerTests : IClassFixture<IntegrationTestWebFactory>, IAsyncLifetime
{
    private readonly IntegrationTestWebFactory _webFactory;

    public GetVolunteersHandlerTests(IntegrationTestWebFactory webFactory)
    {
        _webFactory = webFactory;
    }

    [Fact]
    public async Task HandleAsync_GettingPage1_WithPageSize10_WithSortingByMultipleProperties_ReturnSuccess()
    {
        // Arrange
        IEnumerable<SortByDTO> sort = [
            new SortByDTO(Property: "last_Name", IsAscending: true),
            new SortByDTO(Property: "first_Name", IsAscending: true),
            ];
        var query = new GetVolunteersQuery(
            Sort: sort,
            CurrentPage: 1,
            PageSize: 10);

        var ct = new CancellationTokenSource().Token;

        // database seeding
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        context.Volunteers.AddRange(EntitiesHelper.CreateValidVolunteerList(20));
        await context.SaveChangesAsync();

        var sut = scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery>>();

        // Act
        var result = await sut.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsSuccess);
        var data = result.Value.Data;
        Assert.True(data.Any());
        Assert.True(data.Count() <= 10);
        var firstId = data.First().Id;
        var firstIdFromManuallyOrdered = data
            .OrderBy(v => v.Last_Name)
            .OrderBy(v => v.First_Name)
            .First().Id;

        Assert.True(firstId == firstIdFromManuallyOrdered);
    }

    [Fact]
    public async Task HandleAsync_WhenDataBaseIsEmpty_ReturnSuccess_EmptyList()
    {
        // Arrange
        IEnumerable<SortByDTO> sort = [
            new SortByDTO(Property: "last_Name", IsAscending: true),
            new SortByDTO(Property: "first_Name", IsAscending: true),
            ];
        var query = new GetVolunteersQuery(
            Sort: sort,
            CurrentPage: 1,
            PageSize: 10);

        var ct = new CancellationTokenSource().Token;

        // database seeding
        using var scope = _webFactory.Services.CreateScope();
        var sut = scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery>>();

        // Act
        var result = await sut.HandleAsync(query, ct);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Data.Any() == false);
    }

    public async Task InitializeAsync() { }

    public async Task DisposeAsync()
    {
        await _webFactory.ResetDatabaseAsync();
    }
}
