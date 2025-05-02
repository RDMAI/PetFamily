using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Application.Commands.UpdateVolunteerRequisites;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Kernel.ValueObjects;
using PetFamily.Tests.Shared;

namespace PetFamily.Accounts.Application.Tests;

public class UpdateVolunteerRequisitesHandlerTests : AccountsBaseTests
{
    private readonly ICommandHandler<UpdateVolunteerRequisitesCommand> _sut;

    public UpdateVolunteerRequisitesHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<UpdateVolunteerRequisitesCommand>>();
    }

    [Fact]
    public async Task HandleAsync_UpdateWithValidUserAndRequisites_ReturnSuccess()
    {
        // Arrange
        // seed database
        var validUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "Test",
            Email = "Test@gmail.com",
        };
        _accountContext.Users.Add(validUser);
        
        var role = await _accountContext.Roles.FirstAsync(d => d.NormalizedName == VolunteerAccount.ROLE_NAME);
        _accountContext.UserRoles.Add(new()
        {
            RoleId = role.Id,
            UserId = validUser.Id
        });
        List<Requisites> requisitesBeforeChange = [Requisites.Create("test1", "test1 description", "1111").Value];
        _accountContext.VolunteerAccounts.Add(new VolunteerAccount
        {
            Id = Guid.NewGuid(),
            UserId = validUser.Id,
            Experience = 1,
            Requisites = requisitesBeforeChange
        });

        await _accountContext.SaveChangesAsync();

        var command = new UpdateVolunteerRequisitesCommand(
            UserId: validUser.Id,
            Requisites: [new RequisitesDTO("test2", "test2 description", "2222")]);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);

        var volunteerAccount = await _accountContext.VolunteerAccounts.FirstOrDefaultAsync(d => d.UserId == command.UserId);
        Assert.True(volunteerAccount is not null, "Volunteer account not found");
        
        var requisitesVOAfterChange = Requisites.Create(
            command.Requisites.First().Name,
            command.Requisites.First().Description,
            command.Requisites.First().Value).Value;
        var receivedRequisites = volunteerAccount.Requisites.First();
        Assert.True(
            condition: receivedRequisites == requisitesVOAfterChange,
            userMessage: $"""
            Incorrect requisites.
            Expected name - {requisitesVOAfterChange.Name},
            description - {requisitesVOAfterChange.Description},
            value - {requisitesVOAfterChange.Value}.
            Received name - {receivedRequisites.Name},
            description - {receivedRequisites.Description},
            value - {receivedRequisites.Value}
            """);
    }
}