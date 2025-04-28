using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using PetFamily.Accounts.Contracts;
using System.Security.Claims;

namespace PetFamily.Shared.Framework.Authorization;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionAttribute>
{
    private readonly IServiceScopeFactory _factory;

    public PermissionRequirementHandler(IServiceScopeFactory factory)
    {
        _factory = factory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAttribute permission)
    {
        // get user id from claims
        var idClaim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (idClaim == null || false == Guid.TryParse(idClaim.Value, out Guid userId))
        {
            context.Fail();
            return;
        }
        // check if user with specified id has permissions in db
        using var scope = _factory.CreateScope();
        var accountContract = scope.ServiceProvider.GetRequiredService<IAccountsContract>();

        var result = await accountContract.CheckPermissionByUserIdAsync(userId, permission.Code);
        if (result.IsFailure)
        {
            context.Fail();
            return;
        }

        context.Succeed(permission);
    }
}
