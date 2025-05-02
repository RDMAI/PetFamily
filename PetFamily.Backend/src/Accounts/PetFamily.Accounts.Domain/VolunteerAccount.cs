using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Domain;

public class VolunteerAccount
{
    public const string ROLE_NAME = "VOLUNTEER";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public float Experience { get; set; }
    public List<Requisites> Requisites { get; set; } = [];
}
