using Microsoft.Extensions.DependencyInjection;
using PetFamily.PetsManagement.Infrastructure.Database.Write;
using PetFamily.SpeciesManagement.Infrastructure.Database.Write;
using PetFamily.Tests.Shared;

namespace PetFamily.PetsManagement.Application.Tests;

public class PetsManagementBaseTests : BaseHandlerTests
{
    protected readonly PetsWriteDBContext _petsContext;
    protected readonly SpeciesWriteDBContext _speciesContext;

    public PetsManagementBaseTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _petsContext = _scope.ServiceProvider.GetRequiredService<PetsWriteDBContext>();
        _speciesContext = _scope.ServiceProvider.GetRequiredService<SpeciesWriteDBContext>();
    }

    public override async Task DisposeAsync()
    {
        _petsContext.Dispose();
        _speciesContext.Dispose();
        await base.DisposeAsync();
    }
}
