using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.UpdateUserSocialNetworks;
using PetFamily.Accounts.Application.Commands.UpdateVolunteerRequisites;
using PetFamily.Accounts.Application.DTOs;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Tests.Shared;

namespace PetFamily.Accounts.Application.Tests;

public class UpdateUserSocialNetworksHandlerTests : AccountsBaseTests
{
    private readonly ICommandHandler<UpdateUserSocialNetworksCommand> _sut;

    public UpdateUserSocialNetworksHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<UpdateUserSocialNetworksCommand>>();
    }

    [Fact]
    public async Task HandleAsync_UpdateWithValidUserAndSocialNetworks_ReturnSuccess()
    {
        // Arrange
        // seed database
        List<SocialNetwork> socialNetworksBeforeChange = [SocialNetwork.Create("test1", "https://vk.com/test1").Value];
        var validUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "Test",
            Email = "Test@gmail.com",
            SocialNetworks = socialNetworksBeforeChange
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

        var command = new UpdateUserSocialNetworksCommand(
            UserId: validUser.Id,
            SocialNetworks: [new SocialNetworkDTO("test2", "https://vk.com/test2")]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);

        var user = await _accountContext.Users.FirstOrDefaultAsync(d => d.Id == command.UserId);
        Assert.True(user is not null, "User not found");

        var socialNetworksVOAfterChange = SocialNetwork.Create(
            command.SocialNetworks.First().Name,
            command.SocialNetworks.First().Link).Value;
        var receivedSocialNetworks = user.SocialNetworks.First();
        Assert.True(
            condition: receivedSocialNetworks == socialNetworksVOAfterChange,
            userMessage: $"""
            Incorrect requisites.
            Expected name - {socialNetworksVOAfterChange.Name},
            link - {socialNetworksVOAfterChange.Link}.
            Received name - {receivedSocialNetworks.Name},
            link - {receivedSocialNetworks.Link},
            """);
    }
}