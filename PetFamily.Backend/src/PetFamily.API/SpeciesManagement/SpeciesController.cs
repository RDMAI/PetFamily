using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Shared;
using PetFamily.API.SpeciesManagement.Species.Requests;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;
using PetFamily.Application.SpeciesManagement.Queries.GetSpecies;

namespace PetFamily.API.SpeciesManagement;
public class SpeciesController : ApplicationController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromServices] IQueryHandler<DataListPage<SpeciesDTO>, GetSpeciesQuery> speciesHandler,
        [FromQuery] GetSpeciesRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await speciesHandler.HandleAsync(
            request.ToQuery(),
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("{id:guid}/breeds")]
    public async Task<IActionResult> GetBreeds(
        [FromServices] IQueryHandler<DataListPage<BreedDTO>, GetBreedsQuery> speciesHandler,
        [FromRoute] Guid id,
        [FromQuery] GetBreedsRequest request,
        CancellationToken cancellationToken = default)
    {
        var result = await speciesHandler.HandleAsync(
            request.ToQuery(id),
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }
}
