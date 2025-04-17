using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace PetFamily.Tests.Shared;

public abstract class BaseHandlerTests : IClassFixture<IntegrationTestWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestWebFactory _webFactory;
    protected readonly IServiceScope _scope;

    public BaseHandlerTests(IntegrationTestWebFactory webFactory)
    {
        _webFactory = webFactory;
        _scope = _webFactory.Services.CreateScope();
    }

    public virtual async Task DisposeAsync()
    {
        _scope.Dispose();
        await _webFactory.ResetDatabaseAsync();
    }

    public async Task InitializeAsync() { }
}
