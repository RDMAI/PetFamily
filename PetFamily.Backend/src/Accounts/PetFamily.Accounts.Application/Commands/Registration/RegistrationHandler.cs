using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Commands.Registration;

public class RegistrationHandler
    : ICommandHandler<RegistrationCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RegistrationCommandValidator _validator;
    private readonly ILogger<RegistrationHandler> _logger;

    public RegistrationHandler(
        UserManager<User> userManager,
        RegistrationCommandValidator validator,
        ILogger<RegistrationHandler> logger)
    {
        _userManager = userManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> HandleAsync(
        RegistrationCommand command,
        CancellationToken cancellationToken = default)
    {
        // command validation
        var validatorResult = await _validator.ValidateAsync(command, cancellationToken);
        if (!validatorResult.IsValid)
        {
            var errors = from e in validatorResult.Errors
                         select Error.Deserialize(e.ErrorMessage);
            return new ErrorList(errors);
        }

        var user = new User
        {
            Email = command.Email,
            UserName = command.UserName,
        };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            var mappedErrors = result.Errors.Select(e => Error.Failure(e.Code, e.Description)).ToList();
            return new ErrorList(mappedErrors);
        }

        await _userManager.AddToRoleAsync(user, "Participant");

        _logger.LogInformation("User with email {email} and userName {userName} created", command.Email , command.UserName);

        return UnitResult.Success<ErrorList>();
    }
}
