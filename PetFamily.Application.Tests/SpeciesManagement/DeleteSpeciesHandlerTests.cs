using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Seeding;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.SpeciesManagement.Commands.DeleteBreed;
using PetFamily.Application.SpeciesManagement.Commands.DeleteSpecies;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.SpeciesManagement;

public class DeleteSpeciesHandlerTests : BaseHandlerTests
{
    public DeleteSpeciesHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_DeletingBreedWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var species = SeedingHelper.CreateValidSpecies("Dog");
        context.Species.Add(species);

        await context.SaveChangesAsync();

        var command = new DeleteSpeciesCommand(
            SpeciesId: species.Id.Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<SpeciesId, DeleteSpeciesCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is NOT in the database
        var entity = await context.Species.FirstOrDefaultAsync(s => s.Id == result.Value);
        Assert.True(entity is null, "Entity is not deleted");
    }
}
