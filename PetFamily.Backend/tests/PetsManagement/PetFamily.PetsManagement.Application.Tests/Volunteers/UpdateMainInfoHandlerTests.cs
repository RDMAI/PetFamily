using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Volunteers;

public class UpdateMainInfoHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<VolunteerId, UpdateMainInfoCommand> _sut;

    public UpdateMainInfoHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, UpdateMainInfoCommand>>();
    }

    [Fact]
    public async Task HandleAsync_UpdatingVolunteerFirstNameWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var volunteer = SeedingHelper.CreateValidVolunteer();
        _petsContext.Volunteers.Add(volunteer);
        await _petsContext.SaveChangesAsync();

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

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await _petsContext.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value, ct);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(
            entity.FullName.FirstName == command.FirstName,
            $"Entity first name is incorrect - {entity.FullName.FirstName}, should be {command.FirstName}");
    }
}
