namespace PetFamily.Accounts.Infrastructure.Identity;

public class JWTOptions
{
    public const string CONFIG_NAME = "JWT";

    public string Issuer { get; init; }
    public string Audience { get; init; }
    public string Key { get; init; }
    public int ExpiredMinutesTime { get; init; }
}
