using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.PetsManagement.Application.Volunteers.Queries.GetById;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Volunteers;

public class GetVolunteerByIdHandlerTests : PetsManagementBaseTests
{
    private readonly IQueryHandler<VolunteerDTO, GetVolunteerByIdQuery> _sut;

    public GetVolunteerByIdHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<IQueryHandler<VolunteerDTO, GetVolunteerByIdQuery>>();
    }

    [Fact]
    public async Task HandleAsync_GettingExistingVolunteer_ReturnSuccess()
    {
        // Arrange
        // database seeding
        var volunteers = SeedingHelper.CreateValidVolunteerList(2);
        _petsContext.Volunteers.AddRange(volunteers);
        await _petsContext.SaveChangesAsync();

        var query = new GetVolunteerByIdQuery(volunteers.First().Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(query, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");
        Assert.True(result.Value.Id == query.Id, "Entity with wrong id returned");
    }
}
