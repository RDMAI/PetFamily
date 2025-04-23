using PetFamily.Accounts.Application.DataModels;

namespace PetFamily.Accounts.Application.Interfaces;

public interface ITokenHandler
{
    public string GenerateAccessToken(User user);
}
