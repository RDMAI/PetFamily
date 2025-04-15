using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateRequisites;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Volunteers;

public class UpdateRequisitesHandlerTests : BaseHandlerTests
{
    public UpdateRequisitesHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_UpdatingRequisitesWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        var command = new UpdateRequisitesCommand(
            VolunteerId: volunteer.Id.Value,
            RequisitesList: [new RequisitesDTO(
                Name: "Updated Name",
                Description: "Updated Description",
                Value: "Updated value")]);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, UpdateRequisitesCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await context.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value, ct);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.Requisites.Count != 0, "Requisites list is empty");

        var requisitesFromDB = entity.Requisites[0];
        var requisitesFromCommand = command.RequisitesList.First();
        Assert.True(
            requisitesFromDB.Name == requisitesFromCommand.Name,
            $"Requisites name is incorrect - {requisitesFromDB.Name}, should be {requisitesFromCommand.Name}");
    }
}
