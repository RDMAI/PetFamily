using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.UpdateUserMainInfo;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Tests.Shared;

namespace PetFamily.Accounts.Application.Tests;

public class UpdateUserMainInfoHandlerTests : AccountsBaseTests
{
    private readonly ICommandHandler<UpdateUserMainInfoCommand> _sut;

    public UpdateUserMainInfoHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<UpdateUserMainInfoCommand>>();
    }

    [Fact]
    public async Task HandleAsync_UpdateWithValidUserAndCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var validUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "Test",
            Email = "Test@gmail.com",
            FirstName = "Ivan",
            LastName = "Ivanov",
            FatherName = "Ivanovich"
        };
        _accountContext.Users.Add(validUser);
        
        var role = await _accountContext.Roles.FirstAsync(d => d.NormalizedName == ParticipantAccount.ROLE_NAME);
        _accountContext.UserRoles.Add(new()
        {
            RoleId = role.Id,
            UserId = validUser.Id
        });
        
        _accountContext.ParticipantAccounts.Add(new ParticipantAccount
        {
            Id = Guid.NewGuid(),
            UserId = validUser.Id,
        });

        await _accountContext.SaveChangesAsync();

        var command = new UpdateUserMainInfoCommand(
            UserId: validUser.Id,
            FirstName: "Vasiliy",
            LastName: "Vasiliev",
            FatherName: "Vasilievich");

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);

        var user = await _accountContext.Users.FirstOrDefaultAsync(d => d.Id == command.UserId);
        Assert.True(user is not null, "User not found");

        Assert.True(
            condition: user.FirstName == command.FirstName,
            userMessage: $"""
            Incorrect first name.
            Expected - {command.FirstName}.
            Received - {user.FirstName}
            """);

        Assert.True(
            condition: user.LastName == command.LastName,
            userMessage: $"""
            Incorrect last name.
            Expected - {command.LastName}.
            Received - {user.LastName}
            """);

        Assert.True(
            condition: user.FatherName == command.FatherName,
            userMessage: $"""
            Incorrect father name.
            Expected - {command.FatherName}.
            Received - {user.FatherName}
            """);
    }
}