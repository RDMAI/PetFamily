using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Domain;

public class VolunteerAccount
{
    public int UserId { get; set; }
    public float Experience { get; set; }
    public List<Requisites> Requisites { get; set; } = [];
}
