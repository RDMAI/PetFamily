using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.PetsManagement.Volunteers.Queries.GetById;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Volunteers;

public class GetVolunteerByIdHandlerTests : BaseHandlerTests
{
    public GetVolunteerByIdHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_GettingExistingVolunteer_ReturnSuccess()
    {
        // Arrange
        // database seeding
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        var volunteers = SeedingHelper.CreateValidVolunteerList(2);

        context.Volunteers.AddRange();
        await context.SaveChangesAsync();

        var query = new GetVolunteerByIdQuery(volunteers.First().Id.Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<IQueryHandler<VolunteerDTO, GetVolunteerByIdQuery>>();

        // Act
        var result = await sut.HandleAsync(query, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");
        Assert.True(result.Value.Id == query.Id, "Entity with wrong id returned");
    }
}
