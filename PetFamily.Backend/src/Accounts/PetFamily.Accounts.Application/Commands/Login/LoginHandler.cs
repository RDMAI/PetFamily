using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Commands.Login;

public class LoginHandler
    : ICommandHandler<string, LoginCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly LoginCommandValidator _validator;
    private readonly ILogger<LoginHandler> _logger;
    private readonly ITokenHandler _tokenHandler;

    public LoginHandler(
        UserManager<User> userManager,
        LoginCommandValidator validator,
        ILogger<LoginHandler> logger,
        ITokenHandler tokenHandler)
    {
        _userManager = userManager;
        _validator = validator;
        _logger = logger;
        _tokenHandler = tokenHandler;
    }

    public async Task<Result<string, ErrorList>> HandleAsync(
        LoginCommand command,
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

        var existingUser = await _userManager.FindByNameAsync(command.UserName);
        if (existingUser is null)
            return ErrorHelper.General.NotFound().ToErrorList();

        var passwordConfirmed = await _userManager.CheckPasswordAsync(
            user: existingUser,
            password: command.Password);

        if (!passwordConfirmed)
            return ErrorHelper.Authentication.InvalidLoginAttempt().ToErrorList();

        var accessToken = _tokenHandler.GenerateAccessToken(existingUser);

        _logger.LogInformation("Generated token for user with username {username}", command.UserName);

        return accessToken;
    }
}
