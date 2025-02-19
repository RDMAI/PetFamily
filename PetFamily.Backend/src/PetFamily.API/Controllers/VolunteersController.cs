using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Extensions;
using PetFamily.API.Volunteers.Requests;
using PetFamily.Application.Volunteers.CreateVolunteer;

namespace PetFamily.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VolunteersController : Controller
{
    [HttpPost]
    public async Task<IActionResult> Create([FromServices] CreateVolunteerHandler volunteerHandler, [FromBody] CreateVolunteerRequest request)
    {
        var createCommand = request.ToCommand();
        var result = await volunteerHandler.HandleAsync(createCommand);

        if (result.IsFailure) return result.Error.ToResponse();

        return Ok(result.Value);
    }
}
