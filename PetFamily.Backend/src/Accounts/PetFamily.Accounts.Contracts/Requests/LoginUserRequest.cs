using PetFamily.Accounts.Application.Commands.Login;

namespace PetFamily.Accounts.Contracts.Requests;

public record LoginUserRequest(string UserName, string Password)
{
    public LoginCommand ToCommand()
    {
        return new LoginCommand(UserName, Password);
    }
}
