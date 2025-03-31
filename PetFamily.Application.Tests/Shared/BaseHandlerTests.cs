
namespace PetFamily.Application.IntegrationTests.Shared;

public abstract class BaseHandlerTests : IClassFixture<IntegrationTestWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestWebFactory _webFactory;

    public BaseHandlerTests(IntegrationTestWebFactory webFactory)
    {
        _webFactory = webFactory;
    }

    public async Task DisposeAsync()
    {
        await _webFactory.ResetDatabaseAsync();
    }

    public async Task InitializeAsync() { }
}
