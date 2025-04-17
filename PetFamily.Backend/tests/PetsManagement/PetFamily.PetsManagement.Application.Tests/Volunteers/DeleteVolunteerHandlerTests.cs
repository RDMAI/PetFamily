using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Volunteers.Commands.DeleteVolunteer;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Volunteers;

public class DeleteVolunteerHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<VolunteerId, DeleteVolunteerCommand> _sut;

    public DeleteVolunteerHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, DeleteVolunteerCommand>>();
    }

    [Fact]
    public async Task HandleAsync_SoftDeletingVolunteerWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var volunteer = SeedingHelper.CreateValidVolunteer();
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var command = new DeleteVolunteerCommand(volunteer.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is NOT in the database
        var entity = await _petsContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value, ct);
        Assert.True(entity is not null, "Entity is hard deleted from database, expected soft delete");
        Assert.True(entity.IsDeleted, "Soft delete flag is incorrect");
    }
}
