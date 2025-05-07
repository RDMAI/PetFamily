using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Accounts.Application.Commands.Login;
using PetFamily.Accounts.Application.Commands.RefreshTokens;
using PetFamily.Accounts.Application.Commands.Registration;
using PetFamily.Accounts.Application.Commands.UpdateUserMainInfo;
using PetFamily.Accounts.Application.Commands.UpdateUserSocialNetworks;
using PetFamily.Accounts.Application.Commands.UpdateVolunteerRequisites;
using PetFamily.Accounts.Application.DTOs;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Framework;
using PetFamily.Shared.Framework.Authorization;

namespace PetFamily.Accounts.Presentation;

public class AccountController : ApplicationController
{
    [AllowAnonymous]
    [HttpPost("registration")]
    public async Task<IActionResult> Registration(
        [FromServices] ICommandHandler<RegistrationCommand> handler,
        [FromBody] RegistrationRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return Error(result.Error);

        return Created();
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromServices] ICommandHandler<LoginResponseDTO, LoginCommand> handler,
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return Error(result.Error);

        return Ok(result.Value);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshTokens(
        [FromServices] ICommandHandler<LoginResponseDTO, RefreshTokensCommand> handler,
        [FromBody] RefreshTokensRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return Error(result.Error);

        return Ok(result.Value);
    }

    [Permission(Permissions.Accounts.UPDATE)]
    [HttpPatch("{id:guid}/social-networks")]
    public async Task<IActionResult> UpdateSocialNetworks(
        [FromServices] ICommandHandler<UpdateUserSocialNetworksCommand> volunteerHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateUserSocialNetworksRequest request,
        CancellationToken cancellationToken = default)
    {
        var updateCommand = request.ToCommand(id);
        var result = await volunteerHandler.HandleAsync(updateCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok();
    }

    [Permission(Permissions.Accounts.UPDATE)]
    [HttpPatch("{id:guid}/main-info")]
    public async Task<IActionResult> UpdateMainInfo(
        [FromServices] ICommandHandler<UpdateUserMainInfoCommand> volunteerHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateUserMainInfoRequest request,
        CancellationToken cancellationToken = default)
    {
        var updateCommand = request.ToCommand(id);
        var result = await volunteerHandler.HandleAsync(updateCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok();
    }

    [Permission(Permissions.Accounts.UPDATE)]
    [HttpPatch("{id:guid}/requisites")]
    public async Task<IActionResult> UpdateRequisites(
        [FromServices] ICommandHandler<UpdateVolunteerRequisitesCommand> volunteerHandler,
        [FromRoute] Guid id,
        [FromBody] UpdateVolunteerRequisitesRequest request,
        CancellationToken cancellationToken = default)
    {
        var updateCommand = request.ToCommand(id);
        var result = await volunteerHandler.HandleAsync(updateCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok();
    }
}
