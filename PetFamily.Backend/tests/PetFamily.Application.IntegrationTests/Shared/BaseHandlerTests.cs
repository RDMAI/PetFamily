using Microsoft.Extensions.DependencyInjection;
using PetFamily.Infrastructure.DataBaseAccess.Write;

namespace PetFamily.Application.IntegrationTests.Shared;

public abstract class BaseHandlerTests : IClassFixture<IntegrationTestWebFactory>, IAsyncLifetime
{
    protected readonly IntegrationTestWebFactory _webFactory;
    protected readonly IServiceScope _scope;
    protected readonly WriteDBContext _context;

    public BaseHandlerTests(IntegrationTestWebFactory webFactory)
    {
        _webFactory = webFactory;
        _scope = _webFactory.Services.CreateScope();
        _context = _scope.ServiceProvider.GetRequiredService<WriteDBContext>();
    }

    public async Task DisposeAsync()
    {
        _context.Dispose();
        _scope.Dispose();

        await _webFactory.ResetDatabaseAsync();
    }

    public async Task InitializeAsync() { }
}
