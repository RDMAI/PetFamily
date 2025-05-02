using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.Accounts.Application.Commands.UpdateUserMainInfo;

public record UpdateUserMainInfoCommand(
    Guid UserId,
    string FirstName,
    string LastName,
    string FatherName) : ICommand;
