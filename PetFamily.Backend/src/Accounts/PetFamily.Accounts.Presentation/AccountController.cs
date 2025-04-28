using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Accounts.Application.Commands.Login;
using PetFamily.Accounts.Application.Commands.Registration;
using PetFamily.Accounts.Contracts.Requests;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Framework;

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
        [FromServices] ICommandHandler<string, LoginCommand> handler,
        [FromBody] LoginUserRequest request,
        CancellationToken cancellationToken)
    {
        var command = request.ToCommand();

        var result = await handler.HandleAsync(command, cancellationToken);

        if (result.IsFailure)
            return Error(result.Error);

        return Ok(result.Value);
    }
}
