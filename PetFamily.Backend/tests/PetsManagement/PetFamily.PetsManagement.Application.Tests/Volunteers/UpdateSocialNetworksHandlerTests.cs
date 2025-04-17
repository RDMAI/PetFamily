using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Application.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.PetsManagement.Application.Volunteers.DTOs;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests.Volunteers;

public class UpdateSocialNetworksHandlerTests : PetsManagementBaseTests
{
    private readonly ICommandHandler<VolunteerId, UpdateSocialNetworksCommand> _sut;

    public UpdateSocialNetworksHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, UpdateSocialNetworksCommand>>();
    }

    [Fact]
    public async Task HandleAsync_UpdatingVolunteerSocialNetworkWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var volunteer = SeedingHelper.CreateValidVolunteer();
        _petsContext.Volunteers.Add(volunteer);

        await _petsContext.SaveChangesAsync();

        var command = new UpdateSocialNetworksCommand(
            VolunteerId: volunteer.Id.Value,
            SocialNetworksList: [new SocialNetworkDTO(
                Name: "Updated Name",
                Link: "https://vk.com/id00Updated")]);

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
        Assert.True(entity.SocialNetworks.Count != 0, "SocialNetworks list is empty");

        var socialNetworksFromDB = entity.SocialNetworks[0];
        var socialNetworksFromCommand = command.SocialNetworksList.First();
        Assert.True(
            socialNetworksFromDB.Name == socialNetworksFromCommand.Name,
            $"Requisites name is incorrect - {socialNetworksFromDB.Name}, should be {socialNetworksFromCommand.Name}");
    }
}
