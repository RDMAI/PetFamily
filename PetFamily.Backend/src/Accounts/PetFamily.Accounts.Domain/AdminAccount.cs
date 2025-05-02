namespace PetFamily.Accounts.Domain;

public class AdminAccount
{
    public const string ROLE_NAME = "ADMIN";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
}
