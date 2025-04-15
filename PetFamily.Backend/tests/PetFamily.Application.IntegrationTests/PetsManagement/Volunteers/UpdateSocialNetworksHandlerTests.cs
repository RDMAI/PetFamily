using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.PetsManagement.Volunteers.Commands.CreateVolunteer;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateMainInfo;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateRequisites;
using PetFamily.Application.PetsManagement.Volunteers.Commands.UpdateSocialNetworks;
using PetFamily.Application.PetsManagement.Volunteers.DTOs;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Domain.PetsManagement.ValueObjects.Volunteers;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.PetsManagement.Volunteers;

public class UpdateSocialNetworksHandlerTests : BaseHandlerTests
{
    public UpdateSocialNetworksHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_UpdatingVolunteerSocialNetworkWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var volunteer = SeedingHelper.CreateValidVolunteer();
        context.Volunteers.Add(volunteer);

        await context.SaveChangesAsync();

        var command = new UpdateSocialNetworksCommand(
            VolunteerId: volunteer.Id.Value,
            SocialNetworksList: [new SocialNetworkDTO(
                Name: "Updated Name",
                Link: "https://vk.com/id00Updated")]);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<VolunteerId, UpdateSocialNetworksCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is in the database
        var entity = await context.Volunteers.FirstOrDefaultAsync(v => v.Id == result.Value, ct);
        Assert.True(entity is not null, "Entity from database is null");
        Assert.True(entity.SocialNetworks.Count != 0, "SocialNetworks list is empty");

        var socialNetworksFromDB = entity.SocialNetworks[0];
        var socialNetworksFromCommand = command.SocialNetworksList.First();
        Assert.True(
            socialNetworksFromDB.Name == socialNetworksFromCommand.Name,
            $"Requisites name is incorrect - {socialNetworksFromDB.Name}, should be {socialNetworksFromCommand.Name}");
    }
}
