using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Volunteers.Commands.DeleteVolunteer;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Volunteers;

public class DeleteVolunteerHandlerTests : BaseHandlerTests
{
    public DeleteVolunteerHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_SoftDeletingVolunteerWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        var command = new DeleteVolunteerCommand(volunteer.Id.Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, DeleteVolunteerCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is NOT in the database
        var entity = await context.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value, ct);
        Assert.True(entity is not null, "Entity is hard deleted from database, expected soft delete");
        Assert.True(entity.IsDeleted, "Soft delete flag is incorrect");
    }
}
