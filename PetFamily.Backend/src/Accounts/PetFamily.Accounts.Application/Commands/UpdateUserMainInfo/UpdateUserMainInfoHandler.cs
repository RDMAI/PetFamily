using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;

namespace PetFamily.Accounts.Application.Commands.UpdateUserMainInfo;

public class UpdateUserMainInfoHandler
    : ICommandHandler<UpdateUserMainInfoCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IUserInfoManager _userInfoManager;
    private readonly UpdateUserMainInfoCommandValidator _validator;
    private readonly ILogger<UpdateUserMainInfoHandler> _logger;

    public UpdateUserMainInfoHandler(
        UserManager<User> userManager,
        IUserInfoManager userInfoManager,
        UpdateUserMainInfoCommandValidator validator,
        ILogger<UpdateUserMainInfoHandler> logger)
    {
        _userManager = userManager;
        _userInfoManager = userInfoManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> HandleAsync(
        UpdateUserMainInfoCommand command,
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

        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user is null)
            return ErrorHelper.General.NotFound(command.UserId).ToErrorList();

        return await _userInfoManager.UpdateFullNameAsync(
            user,
            command.FirstName,
            command.LastName,
            command.FatherName,
            cancellationToken);
    }
}
