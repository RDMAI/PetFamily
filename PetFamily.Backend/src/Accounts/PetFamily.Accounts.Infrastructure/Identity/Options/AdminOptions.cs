namespace PetFamily.Accounts.Infrastructure.Identity.Options;

public class AdminOptions
{
    public const string ADMIN = "ADMIN";

    public string Username { get; init; }
    public string Email { get; init; }
    public string Password { get; init; }
}
