using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.Registration;
using PetFamily.Accounts.Domain;
using PetFamily.Accounts.Infrastructure.Identity;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Tests.Shared;

namespace PetFamily.Accounts.Application.Tests;

public class RegistrationHandlerTests : AccountsBaseTests
{
    private readonly ICommandHandler<RegistrationCommand> _sut;

    public RegistrationHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<RegistrationCommand>>();
    }

    [Fact]
    public async Task HandleAsync_RegisteringValidUser_ReturnSuccess()
    {
        // Arrange
        var command = new RegistrationCommand(
            Email: "test@gmail.com",
            UserName: "tester123",
            Password: "string123_A");

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);

        var createdUser = await _accountContext.Users.FirstOrDefaultAsync(d => d.Email == command.Email);
        Assert.True(createdUser is not null, "User is not created");

        var userRole = await _accountContext.UserRoles.FirstOrDefaultAsync(d => d.UserId == createdUser.Id);
        Assert.True(userRole is not null, "User is not added to role");

        var role = await _accountContext.Roles
            .FirstOrDefaultAsync(d => d.Id == userRole.RoleId && d.NormalizedName == ParticipantAccount.ROLE_NAME);
        Assert.True(role is not null, "User is not added to correct role");

        var participantAccount = await _accountContext.ParticipantAccounts
            .FirstOrDefaultAsync(d => d.UserId == createdUser.Id);
        Assert.True(participantAccount is not null, "Participant account for user is not created");
    }
}