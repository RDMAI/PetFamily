namespace PetFamily.Accounts.Domain;

public class ParticipantAccount
{
    public const string ROLE_NAME = "PARTICIPANT";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<Guid> FavouritePets { get; set; } = [];
}
