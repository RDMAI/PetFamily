using Microsoft.AspNetCore.Mvc;
using PetFamily.API.PetsManagement.Volunteers.Requests;
using PetFamily.API.Shared;
using PetFamily.API.Shared.Extensions;
using PetFamily.Application.PetsManagement.Volunteers.CreateVolunteer;

namespace PetFamily.API.PetsManagement.Volunteers;

public class VolunteersController : ApplicationController
{
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromServices] CreateVolunteerHandler volunteerHandler,
        [FromBody] CreateVolunteerRequest request,
        CancellationToken cancellationToken = default)
    {
        var createCommand = request.ToCommand();
        var result = await volunteerHandler.HandleAsync(createCommand, cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        var volunteerId = result.Value;

        return CreatedBaseURI(volunteerId.Value);
    }
}
