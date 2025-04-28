using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetFamily.Shared.Core.Abstractions;
using PetFamily.Shared.Core.DTOs;
using PetFamily.Shared.Framework;
using PetFamily.Shared.Framework.Authorization;
using PetFamily.Shared.Kernel.ValueObjects.Ids;
using PetFamily.SpeciesManagement.Application.Commands.DeleteBreed;
using PetFamily.SpeciesManagement.Application.Commands.DeleteSpecies;
using PetFamily.SpeciesManagement.Application.DTOs;
using PetFamily.SpeciesManagement.Application.Queries.GetBreeds;
using PetFamily.SpeciesManagement.Application.Queries.GetSpecies;
using PetFamily.SpeciesManagement.Contracts.Requests;

namespace PetFamily.SpeciesManagement.Presentation;

[Authorize]
public class SpeciesController : ApplicationController
{
    [Permission(Permissions.Species.READ)]
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

    [Permission(Permissions.Species.DELETE)]
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

    [Permission(Permissions.Breeds.READ)]
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

    [Permission(Permissions.Breeds.DELETE)]
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
