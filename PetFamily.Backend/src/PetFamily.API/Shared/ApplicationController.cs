using Microsoft.AspNetCore.Mvc;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Shared;

[ApiController]
[Route("[controller]")]
public abstract class ApplicationController : Controller
{
    public override OkObjectResult Ok(object? value)
    {
        var envelope = Envelope.Ok(value);

        return base.Ok(envelope);
    }

    /// <summary>
    /// Calls base.Created(uri, value) method with uri for current controller
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [NonAction]
    public CreatedResult CreatedBaseURI(object? value)
    {
        var uri = Request.Path + $"/{value}";

        var envelope = Envelope.Ok(value);

        return base.Created(uri, envelope);
    }

    [NonAction]
    public override CreatedResult Created(string? uri, object? value)
    {
        var envelope = Envelope.Ok(value);

        return base.Created(uri, envelope);
    }

    [NonAction]
    public ActionResult Error(ErrorList errors)
    {
        if (errors == null || !errors.Any())
        {
            throw new ArgumentNullException("List of errors is null");
        }
        
        var statusCode = StatusCodes.Status500InternalServerError;

        switch (errors.First().Type)  // return code of the first error
        {
            case ErrorType.Validation:
                statusCode = StatusCodes.Status400BadRequest;
                break;
            case ErrorType.NotFound:
                statusCode = StatusCodes.Status404NotFound;
                break;
            case ErrorType.Failure:
                statusCode = StatusCodes.Status500InternalServerError;
                break;
            case ErrorType.Conflict:
                statusCode = StatusCodes.Status409Conflict;
                break;
            default:
                break;
        }

        IEnumerable<ResponseError> responseErrors = errors.Select(e => new ResponseError(e.Code, e.Message, e.InvalidField));

        var envelope = Envelope.Error(responseErrors);

        return new ObjectResult(envelope)
        {
            StatusCode = statusCode
        };
    }
}
