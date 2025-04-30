using PetFamily.Accounts.Domain;

namespace PetFamily.Accounts.Application.Interfaces;

public interface ITokenHandler
{
    public string GenerateAccessToken(User user);
}
