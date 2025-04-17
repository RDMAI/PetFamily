using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Volunteers.Commands.CreateVolunteer;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.PetsManagement.Domain.ValueObjects.Volunteers;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Volunteers;

public class CreateVolunteerHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<VolunteerId, CreateVolunteerCommand> _sut;

    public CreateVolunteerHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, CreateVolunteerCommand>>();
    }

    [Fact]
    public async Task HandleAsync_CreatingVolunteerFromValidCommand_ReturnSuccess()
    {
        // Arrange
        var command = new CreateVolunteerCommand(
            FirstName: "Ivan",
            LastName: "Ivanov",
            FatherName: "Ivanovich",
            Email: "Example@gmail.com",
            Description: "test description",
            ExperienceYears: 2,
            Phone: "89000000000",
            RequisitesList: [ new RequisitesDTO(Name: "SPB", Description: "Test SPB description", Value: "89000000000") ],
            SocialNetworksList: [ new SocialNetworkDTO(Name: "VK", Link: "https://vk.com/id000000000")]);

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
    }

    [Fact]
    public async Task HandleAsync_WhenCreatingVolunteerWithExistingEmail_ReturnError()
    {
        // Arrange
        var email = "Example@gmail.com";

        var command = new CreateVolunteerCommand(
            FirstName: "Ivan",
            LastName: "Ivanov",
            FatherName: "Ivanovich",
            Email: email,
            Description: "test description",
            ExperienceYears: 2,
            Phone: "89000000000",
            RequisitesList: [new RequisitesDTO(Name: "SPB", Description: "Test SPB description", Value: "89000000000")],
            SocialNetworksList: [new SocialNetworkDTO(Name: "VK", Link: "https://vk.com/id000000000")]);

        var ct = new CancellationTokenSource().Token;

        // database seeding
        _petsContext.Volunteers.AddRange(SeedingHelper.CreateValidVolunteer(specificFirstName: "Vasya", uniqueEmail: email));
        await _petsContext.SaveChangesAsync();

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure, "Result is success, expected failure");

        // check if the entity is NOT in the database
        var fullName = VolunteerFullName.Create(
            firstName: command.FirstName,
            lastName: command.LastName,
            fatherName: command.FatherName).Value;

        var entity = await _petsContext.Volunteers.FirstOrDefaultAsync(v => v.FullName == fullName, ct);
        Assert.True(entity is null, "Entity from database is not null, expected otherwise");
    }
}
