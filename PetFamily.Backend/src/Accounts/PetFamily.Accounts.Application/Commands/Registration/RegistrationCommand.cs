using PetFamily.Shared.Core.Abstractions;

namespace PetFamily.Accounts.Application.Commands.Registration;

public record RegistrationCommand(
    string Email,
    string UserName,
    string Password) : ICommand;
