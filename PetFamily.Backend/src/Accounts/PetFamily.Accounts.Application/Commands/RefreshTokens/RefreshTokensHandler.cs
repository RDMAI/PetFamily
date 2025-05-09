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

namespace PetFamily.Accounts.Application.Commands.RefreshTokens;

public class RefreshTokensHandler
    : ICommandHandler<LoginResponseDTO, RefreshTokensCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RefreshTokensCommandValidator _validator;
    private readonly ILogger<RefreshTokensHandler> _logger;
    private readonly ITokenManager _tokenManager;
    private readonly IUnitOfWork _unitOfWork;

    public RefreshTokensHandler(
        UserManager<User> userManager,
        RefreshTokensCommandValidator validator,
        ILogger<RefreshTokensHandler> logger,
        ITokenManager tokenManager,
        [FromKeyedServices(DependencyHelper.DependencyKey.Accounts)] IUnitOfWork unitOfWork)
    {
        _userManager = userManager;
        _validator = validator;
        _logger = logger;
        _tokenManager = tokenManager;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<LoginResponseDTO, ErrorList>> HandleAsync(
        RefreshTokensCommand command,
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

        var dataResult = _tokenManager.GetDataFromToken(command.AccessToken);
        if (dataResult.IsFailure)
            return dataResult.Error;

        var user = await _userManager.FindByIdAsync(dataResult.Value.UserId.ToString());
        if (user is null)
            return ErrorHelper.Authentication.UserNotFound().ToErrorList();

        var refreshSessionResult = await _tokenManager.GetRefreshSessionAsync(command.RefreshToken, cancellationToken);
        if (refreshSessionResult.IsFailure)
            return refreshSessionResult.Error;
        var refreshSession = refreshSessionResult.Value;

        var refreshSessionValidationResult = _tokenManager.ValidateRefreshSession(
            accessTokenData: dataResult.Value,
            refreshSession: refreshSessionResult.Value);
        if (refreshSessionValidationResult.IsFailure)
            return refreshSessionValidationResult.Error;

        var transaction = await _unitOfWork.BeginTransaction();

        var oldRefreshSessionDeleteResult = await _tokenManager.DeleteRefreshSessionsAsync(
            [refreshSession],
            cancellationToken);
        if (oldRefreshSessionDeleteResult.IsFailure)
        {
            transaction.Rollback();
            return oldRefreshSessionDeleteResult.Error;
        }

        var refreshSessionCreateResult = await _tokenManager.CreateRefreshSessionAsync(
            user,
            cancellationToken);
        if (refreshSessionCreateResult.IsFailure)
        {
            transaction.Rollback();
            return refreshSessionCreateResult.Error;
        }

        var accessToken = _tokenManager.GenerateAccessToken(user, refreshSessionCreateResult.Value.Jti);

        transaction.Commit();

        _logger.LogInformation("Generated refresh session for user with username {username}", user.UserName);

        return new LoginResponseDTO(accessToken, refreshSession.RefreshToken.ToString());
    }
}
