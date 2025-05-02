using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Application.Commands.UpdateUserSocialNetworks;

public class UpdateUserSocialNetworksHandler
    : ICommandHandler<UpdateUserSocialNetworksCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserInfoManager _userInfoManager;
    private readonly UpdateUserSocialNetworksCommandValidator _validator;
    private readonly ILogger<UpdateUserSocialNetworksHandler> _logger;

    public UpdateUserSocialNetworksHandler(
        UserManager<User> userManager,
        IUserInfoManager userInfoManager,
        UpdateUserSocialNetworksCommandValidator validator,
        ILogger<UpdateUserSocialNetworksHandler> logger)
    {
        _userManager = userManager;
        _userInfoManager = userInfoManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> HandleAsync(
        UpdateUserSocialNetworksCommand command,
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

        var socialNetworks = command.SocialNetworks
            .Select(d => SocialNetwork.Create(d.Name, d.Link).Value)
            .ToList();

        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return ErrorHelper.General.NotFound(command.UserId).ToErrorList();

        return await _userInfoManager.UpdateSocialNetworksAsync(
            user,
            socialNetworks,
            cancellationToken);
    }
}
