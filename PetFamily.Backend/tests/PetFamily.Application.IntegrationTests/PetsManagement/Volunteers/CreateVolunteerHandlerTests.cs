using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Volunteers.Commands.CreateVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Volunteers;

public class CreateVolunteerHandlerTests : BaseHandlerTests
{
    public CreateVolunteerHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

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

        using var scope = _webFactory.Services.CreateScope();
        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, CreateVolunteerCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        var entity = await context.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value, ct);
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
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();
        context.Volunteers.AddRange(SeedingHelper.CreateValidVolunteer(specificFirstName: "Vasya", uniqueEmail: email));
        await context.SaveChangesAsync();
        
        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, CreateVolunteerCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        Assert.True(result.IsFailure, "Result is success, expected failure");

        // check if the entity is NOT in the database
        var fullName = VolunteerFullName.Create(
            firstName: command.FirstName,
            lastName: command.LastName,
            fatherName: command.FatherName).Value;

        var entity = await context.Volunteers.FirstOrDefaultAsync(v => v.FullName == fullName, ct);
        Assert.True(entity is null, "Entity from database is not null, expected otherwise");
    }
}
