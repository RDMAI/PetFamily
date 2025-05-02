using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Accounts.Domain;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using static PetFamily.Shared.Core.DependencyHelper;

namespace PetFamily.Accounts.Application.Commands.Registration;

public class RegistrationHandler
    : ICommandHandler<RegistrationCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly IParticipantManager _participantManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly RegistrationCommandValidator _validator;
    private readonly ILogger<RegistrationHandler> _logger;

    public RegistrationHandler(
        UserManager<User> userManager,
        IParticipantManager participantManager,
        [FromKeyedServices(DependencyKey.Accounts)] IUnitOfWork unitOfWork,
        RegistrationCommandValidator validator,
        ILogger<RegistrationHandler> logger)
    {
        _userManager = userManager;
        _participantManager = participantManager;
        _unitOfWork = unitOfWork;
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

        var transaction = await _unitOfWork.BeginTransaction();

        var userResult = await _userManager.CreateAsync(user, command.Password);

        if (!userResult.Succeeded)
        {
            transaction.Rollback();
            return new ErrorList(
                errors: userResult.Errors.Select(e => Error.Failure(e.Code, e.Description))
                );
        }

        var roleResult = await _userManager.AddToRoleAsync(user, ParticipantAccount.ROLE_NAME);
        if (!roleResult.Succeeded)
        {
            transaction.Rollback();
            return new ErrorList(
                errors: roleResult.Errors.Select(e => Error.Failure(e.Code, e.Description))
                );
        }

        var participantAccount = new ParticipantAccount
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
        };
        var participantResult = await _participantManager.CreateAsync(participantAccount, cancellationToken);
        if (!participantResult.IsSuccess)
        {
            transaction.Rollback();
            return participantResult.Error;
        }

        transaction.Commit();

        _logger.LogInformation("User with email {email} and userName {userName} created", command.Email , command.UserName);

        return UnitResult.Success<ErrorList>();
    }
}
