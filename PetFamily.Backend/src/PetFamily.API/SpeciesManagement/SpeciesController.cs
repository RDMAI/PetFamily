using Microsoft.AspNetCore.Mvc;
using PetFamily.API.Shared;
using PetFamily.API.SpeciesManagement.Species.Requests;
using PetFamily.Application.Shared.Abstractions;
using PetFamily.Application.Shared.DTOs;
using PetFamily.Application.SpeciesManagement.Commands.DeleteBreed;
using PetFamily.Application.SpeciesManagement.Commands.DeleteSpecies;
using PetFamily.Application.SpeciesManagement.DTOs;
using PetFamily.Application.SpeciesManagement.Queries.GetBreeds;
using PetFamily.Application.SpeciesManagement.Queries.GetSpecies;
using PetFamily.Domain.SpeciesManagement.ValueObjects;

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

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        [FromServices] ICommandHandler<SpeciesId, DeleteSpeciesCommand> speciesHandler,
        [FromRoute] Guid id,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteSpeciesCommand(id);

        var result = await speciesHandler.HandleAsync(
            command,
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

    [HttpDelete("{id:guid}/breeds/{breedid:guid}")]
    public async Task<IActionResult> Delete(
        [FromServices] ICommandHandler<BreedId, DeleteBreedCommand> speciesHandler,
        [FromRoute] Guid id,
        [FromRoute] Guid breedid,
        CancellationToken cancellationToken = default)
    {
        var command = new DeleteBreedCommand(id, breedid);

        var result = await speciesHandler.HandleAsync(
            command,
            cancellationToken);

        if (result.IsFailure) return Error(result.Error);

        return Ok(result.Value);
    }
}
