using PetFamily.Accounts.Application.Commands.Registration;

namespace PetFamily.Accounts.Contracts.Requests;

public record RegistrationRequest(
    string Email,
    string UserName,
    string Password)
{
    public RegistrationCommand ToCommand()
    {
        return new RegistrationCommand(Email, UserName, Password);
    }
}
