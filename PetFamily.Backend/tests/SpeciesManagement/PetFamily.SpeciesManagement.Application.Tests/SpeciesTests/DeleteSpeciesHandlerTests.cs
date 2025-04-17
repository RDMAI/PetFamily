using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Application.Commands.DeleteSpecies;
using PetFamily.Tests.Shared;

namespace PetFamily.SpeciesManagement.Application.Tests.SpeciesTests;

public class DeleteSpeciesHandlerTests : SpeciesManagementBaseTests
{
    private readonly ICommandHandler<SpeciesId, DeleteSpeciesCommand> _sut;

    public DeleteSpeciesHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<SpeciesId, DeleteSpeciesCommand>>();
    }

    [Fact]
    public async Task HandleAsync_DeletingBreedWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var species = SeedingHelper.CreateValidSpecies("Dog");
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var command = new DeleteSpeciesCommand(
            SpeciesId: species.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is NOT in the database
        var entity = await _speciesContext.Species.FirstOrDefaultAsync(s => s.Id == result.Value);
        Assert.True(entity is null, "Entity is not deleted");
    }
}
