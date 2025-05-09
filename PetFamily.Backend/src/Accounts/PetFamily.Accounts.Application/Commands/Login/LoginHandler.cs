using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.DTOs;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Commands.Login;

public class LoginHandler
    : ICommandHandler<LoginResponseDTO, LoginCommand>
{
    public const int MAX_REFRESH_SESSIONS = 5;

    private readonly UserManager<User> _userManager;
    private readonly LoginCommandValidator _validator;
    private readonly ILogger<LoginHandler> _logger;
    private readonly ITokenManager _tokenManager;
    private readonly IUnitOfWork _unitOfWork;

    public LoginHandler(
        UserManager<User> userManager,
        LoginCommandValidator validator,
        ILogger<LoginHandler> logger,
        ITokenManager tokenmanager,
        [FromKeyedServices(DependencyHelper.DependencyKey.Accounts)] IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _validator = validator;
        _logger = logger;
        _tokenManager = tokenmanager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponseDTO, ErrorList>> HandleAsync(
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

        

        var refreshSessionsResult = await _tokenManager.GetRefreshSessionsForUserAsync(existingUser.Id, cancellationToken);
        if (refreshSessionsResult.IsFailure)
            return refreshSessionsResult.Error;

        var transaction = await _unitOfWork.BeginTransaction(cancellationToken);

        var refreshSessions = refreshSessionsResult.Value.ToList();

        if (refreshSessions.Count > MAX_REFRESH_SESSIONS)
        {
            var deleteResult = await _tokenManager.DeleteRefreshSessionsAsync(refreshSessions, cancellationToken);
            if (deleteResult.IsFailure)
                return deleteResult.Error;
        }
        
        var refreshSessionResult = await _tokenManager.CreateRefreshSessionAsync(existingUser, cancellationToken);
        if (refreshSessionResult.IsFailure)
        {
            transaction.Rollback();
            return refreshSessionResult.Error;
        }

        var accessToken = _tokenManager.GenerateAccessToken(existingUser, refreshSessionResult.Value.Jti);

        transaction.Commit();

        _logger.LogInformation("Generated token for user with username {username}", command.UserName);

        return new LoginResponseDTO(accessToken, refreshSessionResult.Value.RefreshToken.ToString());
    }
}
