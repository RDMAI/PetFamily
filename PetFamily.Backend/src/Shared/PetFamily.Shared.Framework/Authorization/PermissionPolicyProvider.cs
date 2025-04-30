using Microsoft.AspNetCore.Authorization;

namespace PetFamily.Shared.Framework.Authorization;

public class PermissionPolicyProvider : IAuthorizationPolicyProvider
{
    public async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (string.IsNullOrEmpty(policyName))
            return null;

        return new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .AddRequirements(new PermissionAttribute(policyName))
            .Build();
    }

    public async Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();

    public async Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => null;
}
