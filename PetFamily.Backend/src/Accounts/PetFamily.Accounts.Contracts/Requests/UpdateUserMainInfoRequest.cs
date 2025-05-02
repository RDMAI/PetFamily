using PetFamily.Accounts.Application.Commands.UpdateUserMainInfo;

namespace PetFamily.Accounts.Contracts.Requests;

public record UpdateUserMainInfoRequest(
    string FirstName,
    string LastName,
    string FatherName)
{
    public UpdateUserMainInfoCommand ToCommand(Guid userId)
    {
        return new UpdateUserMainInfoCommand(
            userId,
            FirstName,
            LastName,
            FatherName);
    }
}