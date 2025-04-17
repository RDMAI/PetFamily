using Microsoft.Extensions.DependencyInjection;
using PetFamily.SpeciesManagement.Infrastructure.Database.Write;
using PetFamily.Tests.Shared;

namespace PetFamily.SpeciesManagement.Application.Tests;

public class SpeciesManagementBaseTests : BaseHandlerTests
{
    protected readonly SpeciesWriteDBContext _speciesContext;

    public SpeciesManagementBaseTests(IntegrationTestWebFactory webFactory) : base(webFactory)
    {
        _speciesContext = _scope.ServiceProvider.GetRequiredService<SpeciesWriteDBContext>();
    }

    public override async Task DisposeAsync()
    {
        _speciesContext.Dispose();
        await base.DisposeAsync();
    }
}
