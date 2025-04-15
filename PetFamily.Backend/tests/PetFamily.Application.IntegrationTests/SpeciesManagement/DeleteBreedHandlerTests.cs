using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Application.IntegrationTests.Shared;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.SpeciesManagement.Commands.DeleteBreed;
using PetFamily.Domain.SpeciesManagement.ValueObjects;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.SpeciesManagement;

public class DeleteBreedHandlerTests : BaseHandlerTests
{
    public DeleteBreedHandlerTests(IntegrationTestWebFactory webFactory) : base(webFactory) { }

    [Fact]
    public async Task HandleAsync_DeletingBreedWithValidCommand_ReturnSuccess()
    {
        // Arrange
        // seed database
        using var scope = _webFactory.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WriteDBContext>();

        var species = SeedingHelper.CreateValidSpecies("Dog");
        var breed = SeedingHelper.CreateValidBreed("Labrador");
        species.AddBreed(breed);

        context.Species.Add(species);

        await context.SaveChangesAsync();

        var command = new DeleteBreedCommand(
            SpeciesId: species.Id.Value,
            BreedId: breed.Id.Value);

        var ct = new CancellationTokenSource().Token;

        var sut = scope.ServiceProvider
            .GetRequiredService<ICommandHandler<BreedId, DeleteBreedCommand>>();

        // Act
        var result = await sut.HandleAsync(command, ct);

        // Assert
        var errorMessage = result.IsSuccess ? "" : result.Error.First().Message;
        Assert.True(result.IsSuccess, errorMessage);
        Assert.True(result.Value is not null, "Result value is null");

        // check if the entity is NOT in the database
        var entity = await context.Species.Where(s => s.Breeds.Any(p => p.Id == result.Value)).FirstOrDefaultAsync();
        Assert.True(entity is null, "Entity is not deleted");
    }
}
