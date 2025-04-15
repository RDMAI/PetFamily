using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetVolunteers;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Volunteers;

public class GetVolunteersHandlerTests : BaseHandlerTests
{
    public GetVolunteersHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

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
        context.Volunteers.AddRange(SeedingHelper.CreateValidVolunteerList(20));
        await context.SaveChangesAsync();

        var sut = scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery>>();

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
            .OrderBy(v => v.Last_Name)
            .OrderBy(v => v.First_Name)
            .First().Id;

        Assert.True(firstId == firstIdFromManuallyOrdered, "Data ordering is incorrect");
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
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value.Data.Any() == false, "Data from database is not empty, expected otherwise");
    }
}
