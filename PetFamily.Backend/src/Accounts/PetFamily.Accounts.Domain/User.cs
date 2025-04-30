using Microsoft.AspNetCore.Identity;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Domain;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public List<SocialNetwork> SocialNetworks { get; set; } = [];
}
