using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateRequisites;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Volunteers;

public class UpdateRequisitesHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<VolunteerId, UpdateRequisitesCommand> _sut;

    public UpdateRequisitesHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, UpdateRequisitesCommand>>();
    }

    [Fact]
    public async Task HandleAsync_UpdatingRequisitesWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var volunteer = SeedingHelper.CreateValidVolunteer();
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

        var command = new UpdateRequisitesCommand(
            VolunteerId: volunteer.Id.Value,
            RequisitesList: [new RequisitesDTO(
                Name: "Updated Name",
                Description: "Updated Description",
                Value: "Updated value")]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await _petsContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value, ct);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.Requisites.Count != 0, "Requisites list is empty");

        var requisitesFromDB = entity.Requisites[0];
        var requisitesFromCommand = command.RequisitesList.First();
        Assert.True(
            requisitesFromDB.Name == requisitesFromCommand.Name,
            $"Requisites name is incorrect - {requisitesFromDB.Name}, should be {requisitesFromCommand.Name}");
    }
}
