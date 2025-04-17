using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Application.Commands.DeleteBreed;
using PetFamily.Tests.Shared;

namespace PetFamily.SpeciesManagement.Application.Tests.Breeds;

public class DeleteBreedHandlerTests : SpeciesManagementBaseTests
{
    private readonly ICommandHandler<BreedId, DeleteBreedCommand> _sut;

    public DeleteBreedHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _sut = _scope.ServiceProvider
            .GetRequiredService<ICommandHandler<BreedId, DeleteBreedCommand>>();
    }

    [Fact]
    public async Task HandleAsync_DeletingBreedWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        var species = SeedingHelper.CreateValidSpecies("Dog");
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        species.AddBreed(breed);
        _speciesContext.Species.Add(species);
        await _speciesContext.SaveChangesAsync();

        var command = new DeleteBreedCommand(
            SpeciesId: species.Id.Value,
            BreedId: breed.Id.Value);

        var ct = new CancellationTokenSource().Token;

        // Act
        var result = await _sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is NOT in the database
        var entity = await _speciesContext.Species.Where(s => s.Breeds.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is null, "Entity is not deleted");
    }
}
