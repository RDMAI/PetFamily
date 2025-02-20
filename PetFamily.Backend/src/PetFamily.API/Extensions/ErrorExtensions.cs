using Microsoft.AspNetCore.Mvc;
using PetFamily.Domain.Shared;

namespace PetFamily.API.Extensions;

public static class ErrorExtensions
{
    public static ActionResult ToResponse(this Error error)
    {
        var statusCode = StatusCodes.Status500InternalServerError;
        switch (error.Type)
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


        return new ObjectResult(error)
        {
            StatusCode = statusCode
        };
    }
}
