using PetFamily.Accounts.Application.Commands.RefreshTokens;

namespace PetFamily.Accounts.Contracts.Requests;

public record RefreshTokensRequest(string AccessToken, string RefreshToken)
{
    public RefreshTokensCommand ToCommand()
    {
        return new RefreshTokensCommand(AccessToken, RefreshToken);
    }
}
