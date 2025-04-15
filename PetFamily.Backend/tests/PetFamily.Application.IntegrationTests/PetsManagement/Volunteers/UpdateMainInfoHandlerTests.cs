using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Volunteers;

public class UpdateMainInfoHandlerTests : BaseHandlerTests
{
    public UpdateMainInfoHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_UpdatingVolunteerFirstNameWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        var command = new UpdateMainInfoCommand(
            VolunteerId: volunteer.Id.Value,
            FirstName: "UpdatedName",
            LastName: volunteer.FullName.LastName,
            FatherName: volunteer.FullName.FatherName,
            Email: volunteer.Email.Value,
            Description: volunteer.Description.Value,
            ExperienceYears: volunteer.ExperienceYears.Value,
            Phone: volunteer.Phone.Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, UpdateMainInfoCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await context.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value, ct);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(
            entity.FullName.FirstName == command.FirstName,
            $"Entity first name is incorrect - {entity.FullName.FirstName}, should be {command.FirstName}");
    }
}
