using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.PetsManagement.Application.Volunteers.Queries.GetVolunteers;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Volunteers;

public class GetVolunteersHandlerTests : PetsManagementBaseTests
{
    private readonly IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery> _sut;

    public GetVolunteersHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<DataListPage<VolunteerDTO>, GetVolunteersQuery>>();
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
        _petsContext.Volunteers.AddRange(SeedingHelper.CreateValidVolunteerList(20));
        await _petsContext.SaveChangesAsync();

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

        // Act
        var result = await _sut.HandleAsync(query, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value.Data.Any() == false, "Data from database is not empty, expected otherwise");
    }
}
