using PetFamily.Shared.Kernel.ValueObjects.Ids;

namespace PetFamily.Accounts.Domain;

public class ParticipantAccount
{
    public Guid UserId { get; set; }
    public List<PetId> FavouritePets { get; set; } = [];
}
