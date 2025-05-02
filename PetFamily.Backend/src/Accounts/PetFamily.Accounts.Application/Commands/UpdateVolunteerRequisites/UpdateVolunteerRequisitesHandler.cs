using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using PetFamily.Accounts.Application.Interfaces;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Kernel;
using PetFamily.Shared.Kernel.ValueObjects;

namespace PetFamily.Accounts.Application.Commands.UpdateVolunteerRequisites;

public class UpdateVolunteerRequisitesHandler
    : ICommandHandler<UpdateVolunteerRequisitesCommand>
{
    private readonly IVolunteerManager _volunteerManager;
    private readonly UpdateVolunteerRequisitesCommandValidator _validator;
    private readonly ILogger<UpdateVolunteerRequisitesHandler> _logger;

    public UpdateVolunteerRequisitesHandler(
        IVolunteerManager volunteerManager,
        UpdateVolunteerRequisitesCommandValidator validator,
        ILogger<UpdateVolunteerRequisitesHandler> logger)
    {
        _volunteerManager = volunteerManager;
        _validator = validator;
        _logger = logger;
    }

    public async Task<UnitResult<ErrorList>> HandleAsync(
        UpdateVolunteerRequisitesCommand command,
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

        var requisites = command.Requisites
            .Select(d => Requisites.Create(d.Name, d.Description, d.Value).Value)
            .ToList();

        var volunteerResult = await _volunteerManager.GetByUserIdAsync(command.UserId);
        if (volunteerResult.IsFailure)
            return ErrorHelper.General.NotFound(command.UserId).ToErrorList();

        return await _volunteerManager.UpdateRequisites(
            volunteerResult.Value,
            requisites,
            cancellationToken);
    }
}
