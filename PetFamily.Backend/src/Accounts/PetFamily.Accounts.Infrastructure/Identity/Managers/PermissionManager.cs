using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Infrastructure.Identity.Managers;

public class PermissionManager : IPermissionManager
{
    private readonly AccountDBContext _context;

    public PermissionManager(AccountDBContext context)
    {
        _context = context;
    }

    public async Task<UnitResult<ErrorList>> CheckPermissionByUserIdAsync(
        Guid userId,
        string permissionCode,
        CancellationToken cancellationToken = default)
    {
        var specifiedPermission = await (
            from p in _context.Permissions
            join rp in _context.RolePermissions on p.Id equals rp.PermissionId
            join ur in _context.UserRoles on rp.RoleId equals ur.RoleId
            where ur.UserId == userId && p.Code == permissionCode
            select p
            ).FirstOrDefaultAsync(cancellationToken);

        if (specifiedPermission is null)
            return ErrorHelper.Authorization.AccessDenied().ToErrorList();

        return UnitResult.Success<ErrorList>();
    }
}
